using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    [DebuggerDisplay("{StartupTaskKey}:{IsVisited},{VisitedFinishTime}")]
    internal class StartupTaskWrapper : IStartupTaskWrapper
    {
        private readonly StartupManagerBase _manager;
        public HashSet<string> FollowTasks { get; private set; } = new HashSet<string>();
        public HashSet<string> Dependencies { get; private set; } = new HashSet<string>();

        public string StartupTaskKey { get; }

        internal VisitState IsVisited { get; set; } = VisitState.Unvisited;

        internal int VisitedFinishTime { get; set; } = 0;

        public StartupCategory Categories { get; internal set; } = StartupCategory.All;

        public StartupTaskBase TaskBase { get; internal set; }
            // 框架注入，一定不为空
            = null!;
        public bool UIOnly { get; internal set; }
        public StartupCriticalLevel CriticalLevel { get; set; }

        public StartupTaskWrapper(string startupTaskKey, StartupManagerBase manager)
        {
            _manager = manager;
            StartupTaskKey = startupTaskKey;
        }

        public async void ExecuteTask(IEnumerable<StartupTaskBase> dependencies, StartupContext context)
        {
            await Task.WhenAll(dependencies.Select(task => task.TaskResult));
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            context.Logger.RecordDuration(StartupTaskKey,
                async () =>
                {
                    try
                    {
                        if (CriticalLevel == StartupCriticalLevel.Critical)
                        {
                            //todo Tracer.Info($"[Startup]关键节点：{StartupTaskKey}开始执行");
                        }

                        var result = await _manager.ExecuteStartupTaskAsync(TaskBase, context, UIOnly);

                        if (CriticalLevel == StartupCriticalLevel.Critical)
                        {
                            //todo Tracer.Info($"[Startup]关键节点：{StartupTaskKey}执行完成");
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        if (CriticalLevel == StartupCriticalLevel.Critical)
                        {
                            Trace.WriteLine(ex.ToString());
                            await context.FastFail(ex);
                        }
                        else
                        {
                            try
                            {
                                //todo Tracer.Error(ex);
                            }
                            catch
                            {
                                // 由于目前正处于启动期间，所以日志模块可能并未真正完成初始化。
                                // 实际上日志模块一定初始化完毕了，因为日志之前的启动异常都会进入上面的 Critical 分支。
                            }

                            //todo Trace.WriteLine(ex.ToString());
#if DEBUG
                            // 启动过程中非Critical级别的启动项出现异常，虽然不影响启动，但也应需要修复
                            Debugger.Break();
#endif
                        }

                        return "";
                    }
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }
    }
}
