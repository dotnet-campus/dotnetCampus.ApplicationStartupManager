using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using dotnetCampus.Configurations.Core;

namespace dotnetCampus.ApplicationStartupManager
{
    public class StartupManagerBase : IStartupManager
    {
        private readonly IMainThreadDispatcher _dispatcher;

        ///// <summary>
        ///// Builder 模式所需状态：包含当前剩余需要管理的启动任务程序集。
        ///// </summary>
        //private readonly List<Assembly> _assembliesToBeManaged = new List<Assembly>();

        /// <summary>
        /// Builder 模式所需状态：包含当前所有的关键启动任务。
        /// </summary>
        private readonly List<StartupTaskWrapper> _criticalTasks = new List<StartupTaskWrapper>();

        /// <summary>
        /// Builder 模式所需状态：包含当前所有的关键启动任务。
        /// </summary>
        private readonly List<Action<StartupTaskBuilder>> _additionalBuilders = new List<Action<StartupTaskBuilder>>();

        /// <summary>
        /// Builder 模式所需状态：用于决定只有哪一些启动任务才是有效的。
        /// </summary>
        private StartupCategory _selectingCategories = StartupCategory.All;

        private readonly int _workerThreads;
        private readonly int _completionPortThreads;

        internal ConcurrentDictionary<string, StartupTaskWrapper> StartupTaskWrappers { get; } =
            new ConcurrentDictionary<string, StartupTaskWrapper>();

        private List<StartupTaskWrapper>? Graph { get; set; }

        private StartupContext Context { get; }
        protected IStartupContext StartupContext => Context;

        protected IStartupLogger Logger => Context.Logger;

        public StartupManagerBase(IStartupLogger logger, /*FileConfigurationRepo configurationRepo,*/
            Func<Exception, Task> fastFailAction, IMainThreadDispatcher dispatcher, bool shouldSetThreadPool = true)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            //if (configurationRepo is null)
            //{
            //    throw new ArgumentNullException(nameof(configurationRepo));
            //}

            _dispatcher = dispatcher;

            if (shouldSetThreadPool)
            {
                ThreadPool.GetMinThreads(out _workerThreads, out _completionPortThreads);
                //启动期间存在大量的线程池调用（包含IO操作），而创建的多数线程在等待 IO 时都是不会被调度的
                //设置更多的初始化线程数可以减少启动期间的线程调度等待
                ThreadPool.SetMinThreads(Math.Max(_workerThreads, 16), Math.Max(_completionPortThreads, 16));
            }

            Context = new StartupContext(logger, /*configurationRepo,*/
                fastFailAction, WaitStartupTaskAsync);
            Logger.RecordTime("ManagerInitialized");
        }

        ///// <summary>
        ///// 配置被 <see cref="StartupManager"/> 管理的程序集。
        ///// 只有被管理的程序集中的启动信息、依赖注入信息才会被执行。
        ///// </summary>
        ///// <param name="assemblies"></param>
        ///// <returns></returns>
        //public StartupManagerBase ConfigAssemblies(IEnumerable<Assembly> assemblies)
        //{
        //    // 可能的限制尚未完成：
        //    // 1. Run 之后不能再调用此方法（适用于固定的程序集应用）；
        //    // 2. 可以多次 Config 然后多次 Run（适用于动态加载的插件程序集）。
        //    _assembliesToBeManaged.AddRange(assemblies);
        //    return this;
        //}

        /// <summary>
        /// 配置被 <see cref="StartupManagerBase"/> 管理的程序集。
        /// 只有被管理的程序集中的启动信息、依赖注入信息才会被执行。
        /// </summary>
        /// <param name="collector"></param>
        /// <returns></returns>
        public virtual StartupManagerBase AddStartupTaskMetadataCollector(Func<IEnumerable<StartupTaskMetadata>> collector)
        {
            // 可能的限制尚未完成：
            // 1. Run 之后不能再调用此方法（适用于固定的程序集应用）；
            // 2. 可以多次 Config 然后多次 Run（适用于动态加载的插件程序集）。
            _startupTaskMetadataCollectorList.Add(collector);
            return this;
        }

        private readonly ConcurrentBag<Func<IEnumerable<StartupTaskMetadata>>> _startupTaskMetadataCollectorList = new ConcurrentBag<Func<IEnumerable<StartupTaskMetadata>>>();

        /// <summary>
        /// 在启动流程中定义一组关键的启动节点。使用此方法定义的关键启动节点将按顺序前后依次依赖。
        /// 例如传入 A、B、C、D 四个关键启动节点，那么 A - B - C - D 将依次执行，其他任务将插入其中。
        /// </summary>
        /// <param name="criticalNodeKeys">关键启动节点的名称。</param>
        /// <returns><see cref="StartupManagerBase"/> 实例自身，用于使用重建者模式创建启动流程管理器。</returns>
        /// <remarks>
        /// <para>
        /// 通常情况下你可以编写常规的 StartupTask 来添加一个关键节点，只要这个节点被众多节点声明了依赖，那么它就能视为一个关键节点。
        /// 使用此方法可以添加一些虚拟的，实际上不会执行任何启动任务的关键启动节点，用于汇总其他模块的依赖关系。
        /// </para>
        /// </remarks>
        public StartupManagerBase UseCriticalNodes(params string[] criticalNodeKeys)
        {
            if (criticalNodeKeys == null)
            {
                throw new ArgumentNullException(nameof(criticalNodeKeys));
            }

            foreach (var wrapper in EnumerateCreate())
            {
                if (_criticalTasks.Find(x => x.StartupTaskKey == wrapper.StartupTaskKey) != null)
                {
                    throw new ArgumentException($@"不能重复添加 {wrapper.StartupTaskKey} 的关键启动任务。", nameof(criticalNodeKeys));
                }

                _criticalTasks.Add(wrapper);
            }

            return this;

            IEnumerable<StartupTaskWrapper> EnumerateCreate()
            {
                for (var i = 0; i < criticalNodeKeys.Length; i++)
                {
                    var key = criticalNodeKeys[i];
                    var current = GetStartupTaskWrapper(key);
                    current.TaskBase = new NullObjectStartup();
                    current.Categories = StartupCategory.All;

                    if (i - 1 >= 0)
                    {
                        current.Dependencies.Add(criticalNodeKeys[i - 1]);
                    }

                    if (i + 1 <= criticalNodeKeys.Length - 1)
                    {
                        current.FollowTasks.Add(criticalNodeKeys[i + 1]);
                    }

                    yield return current;
                }
            }
        }

        /// <summary>
        /// 向启动流程中再额外添加一个关键启动节点。
        /// </summary>
        /// <param name="nodeName">关键节点的名称。</param>
        /// <param name="beforeTasks">关键节点的前置节点。</param>
        /// <param name="afterTasks">关键节点的后置节点。</param>
        /// <returns><see cref="StartupManagerBase"/> 实例自身，用于使用重建者模式创建启动流程管理器。</returns>
        /// <remarks>
        /// <para>
        /// 通常情况下你可以编写常规的 StartupTask 来添加一个关键节点，只要这个节点被众多节点声明了依赖，那么它就能视为一个关键节点。
        /// 但是，使用 StartupTask 的方式生成的关键节点是静态的，在一个版本的程序集已完成之后便不可再被修改。
        /// 你可以使用此方法创建动态的关键启动节点。
        /// </para>
        /// <para>
        /// 例如，你需要根据不同的启动条件决定不同的启动顺序，那么你可能需要使用此方法动态生成关键节点。
        /// </para>
        /// </remarks>
        public StartupManagerBase AddCriticalNodes(string nodeName, string? beforeTasks = null, string? afterTasks = null)
        {
            var wrapper = GetStartupTaskWrapper(nodeName);
            wrapper.TaskBase = new NullObjectStartup();
            wrapper.Categories = StartupCategory.All;

            if (beforeTasks?.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries) is { } before)
            {
                foreach (var b in before)
                {
                    wrapper.FollowTasks.Add(b);
                    GetStartupTaskWrapper(b).Dependencies.Add(nodeName);
                }
            }

            if (afterTasks?.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries) is { } after)
            {
                foreach (var a in after)
                {
                    wrapper.Dependencies.Add(a);
                    GetStartupTaskWrapper(a).FollowTasks.Add(nodeName);
                }
            }

            _criticalTasks.Add(wrapper);
            return this;
        }

        // 不支持被使用
        //public StartupManagerBase SelectNodes(StartupCategory categories)
        //{
        //    _selectingCategories = categories;
        //    return this;
        //}

        public StartupManagerBase ForStartupTasksOfCategory(Action<StartupTaskBuilder> taskBuilder)
        {
            _additionalBuilders.Add(taskBuilder);
            return this;
        }

        public async void Run()
        {
            if (Graph == null)
            {
                Graph = BuildStartupGraph();
                Logger.RecordTime("GraphBuilded");
            }

            var dispatcher = _dispatcher;
            foreach (var wrapper in Graph)
            {
                var startupTasks = wrapper.Dependencies.Select(s => GetStartupTaskWrapper(s).TaskBase);
                if (wrapper.UIOnly)
                {
                    await dispatcher.InvokeAsync(() => wrapper.ExecuteTask(startupTasks, Context));
                }
                else
                {
                    wrapper.ExecuteTask(startupTasks, Context);
                }
            }

            await Graph.Last().TaskBase.TaskResult;

            Logger.RecordTime("AllStartupTasksCompleted");

            Debug.WriteLine(Logger);
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            dispatcher.InvokeAsync(() =>
                Logger.ReportResult(Graph.OfType<IStartupTaskWrapper>().ToList()));
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

            ThreadPool.SetMinThreads(Math.Max(_workerThreads, 8), Math.Max(_completionPortThreads, 8));
        }

        private List<StartupTaskWrapper> BuildStartupGraph()
        {
            var wrappers = _criticalTasks.ToList();
            foreach (var wrapper in wrappers)
            {
                wrapper.Categories &= _selectingCategories;
            }

            var taskMetadataList = ExportStartupTasks().ToList();

            foreach (var meta in taskMetadataList)
            {
                var wrapper = GetStartupTaskWrapper(meta.Key);
                wrapper.UIOnly = meta.Scheduler == StartupScheduler.UIOnly;
                wrapper.Categories = meta.Categories;
                wrapper.CriticalLevel = meta.CriticalLevel;
                wrapper.TaskBase = meta.Instance;
                wrapper.TaskBase.Manager = this;
                wrappers.Add(wrapper);
            }

            foreach (var wrapper in wrappers)
            {
                // 对于预设的启动任务，在此处先执行构造。
                var taskBuilder = new StartupTaskBuilder(wrapper, AddDependencies, AddFollowTasks);
                foreach (var builder in _additionalBuilders)
                {
                    builder(taskBuilder);
                }
            }

            foreach (var meta in taskMetadataList)
            {
                var wrapper = GetStartupTaskWrapper(meta.Key);

                if (meta.AfterTasks != null)
                {
                    AddDependencies(wrapper, meta.AfterTasks);
                }

                if (meta.BeforeTasks != null)
                {
                    AddFollowTasks(wrapper, meta.BeforeTasks);
                }
            }

            return DFSGraph(wrappers);

            void AddDependencies(StartupTaskWrapper wrapper, string afterTasks)
            {
                foreach (var task in afterTasks.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(CompatibleTaskName))
                {
                    if (!taskMetadataList.Exists(metadata => metadata.Key == task)
                        && !wrappers.Exists(taskWrapper => taskWrapper.StartupTaskKey == task))
                    {
                        throw new InvalidOperationException($"该启动流程{wrapper.StartupTaskKey}的依赖项{task}未加入到该Category");
                    }

                    var dependentWrappers = GetStartupTaskWrapper(task);
                    if (dependentWrappers.Categories.HasFlag(wrapper.Categories))
                    {
                        dependentWrappers.FollowTasks.Add(wrapper.StartupTaskKey);
                        wrapper.Dependencies.Add(task);
                    }
                }
            }

            void AddFollowTasks(StartupTaskWrapper wrapper, string beforeTasks)
            {
                foreach (var task in beforeTasks.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(CompatibleTaskName))
                {
                    if (!taskMetadataList.Exists(metadata => metadata.Key == task)
                        && !wrappers.Exists(taskWrapper => taskWrapper.StartupTaskKey == task))
                    {
                        continue;
                    }

                    var followedWrappers = GetStartupTaskWrapper(task);
                    if (wrapper.Categories.HasFlag(followedWrappers.Categories))
                    {
                        wrapper.FollowTasks.Add(task);
                        followedWrappers.Dependencies.Add(wrapper.StartupTaskKey);
                    }
                }
            }
        }

        protected virtual IEnumerable<StartupTaskMetadata> ExportStartupTasks()
        {
            foreach (var func in _startupTaskMetadataCollectorList)
            {
                var taskMetadataList = func();
                foreach (var taskMetadata in taskMetadataList)
                {
                    yield return taskMetadata;
                }
            }
        }

        private static string CompatibleTaskName(string task)
        {
            const string startupName = "Startup";

            if (task.EndsWith(startupName))
            {
                return task.Remove(task.Length - startupName.Length, startupName.Length);
            }
            else
            {
                return task;
            }
        }

        // ReSharper disable once InconsistentNaming
        private List<StartupTaskWrapper> DFSGraph(List<StartupTaskWrapper> wrappers)
        {
            var time = 0;
            wrappers.ForEach(wrapper =>
            {
                if (wrapper.IsVisited == VisitState.Visiting)
                {
                    throw new InvalidOperationException("深度优先遍历出错");
                }

                if (wrapper.IsVisited == VisitState.Unvisited)
                {
                    time = DFSVisit(wrapper, time);
                }
            });
            var topologicalList = wrappers.OrderByDescending(wrapper => wrapper.VisitedFinishTime).ToList();
            return topologicalList;
        }

        // ReSharper disable once InconsistentNaming
        private int DFSVisit(StartupTaskWrapper visitWrapper, int startTime)
        {
            var time = startTime + 1;
            visitWrapper.IsVisited = VisitState.Visiting;

            foreach (var s in visitWrapper.FollowTasks)
            {
                var wrapper = GetStartupTaskWrapper(s);
                if (wrapper.IsVisited == VisitState.Visiting)
                {
                    throw new InvalidOperationException($"启动序列图存在环：{wrapper.StartupTaskKey}");
                }

                if (wrapper.IsVisited == VisitState.Unvisited)
                {
                    time = DFSVisit(wrapper, time);
                }
            }

            visitWrapper.VisitedFinishTime = time + 1;
            visitWrapper.IsVisited = VisitState.Visited;
            return visitWrapper.VisitedFinishTime;
        }

        internal StartupTaskWrapper GetStartupTaskWrapper(string startupTaskKey)
        {
            if (StartupTaskWrappers.TryGetValue(startupTaskKey, out var wrapper))
            {
                return wrapper;
            }

            wrapper = new StartupTaskWrapper(startupTaskKey, this);
            if (StartupTaskWrappers.TryAdd(startupTaskKey, wrapper))
            {
                return wrapper;
            }

            if (StartupTaskWrappers.TryGetValue(startupTaskKey, out wrapper))
            {
                return wrapper;
            }

            throw new InvalidOperationException($"{startupTaskKey}既无法添加至字典，也无法从字典获取对应值");
        }

        public Task WaitStartupTaskAsync(string startupTaskKey)
            => GetStartupTaskWrapper(startupTaskKey).TaskBase.TaskResult;

        StartupTaskBase IStartupManager.GetStartupTask<T>()
            => GetStartupTaskWrapper(StartupTypeToKey(typeof(T))).TaskBase;

        private static string StartupTypeToKey(Type type)
            => type.Name.Remove(type.Name.Length - "startup".Length);

        protected internal virtual Task<string> ExecuteStartupTaskAsync(StartupTaskBase startupTask, IStartupContext context, bool uiOnly)
        {
            return startupTask.JoinAsync(context, uiOnly);
        }
    }
}
