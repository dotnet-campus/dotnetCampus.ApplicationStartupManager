using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动任务项的基类型
    /// </summary>
    public abstract class StartupTaskBase
    {
        /// <summary>
        /// 将当前启动任务项加入执行
        /// </summary>
        /// <remarks>
        /// 此函数由启动框架调用
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="isUIOnly"></param>
        /// <returns></returns>
        /// 由于我们都在编译期间收集 Attribute 了，当然也能收集使用方到底重写了哪个 Run。
        /// 这里传入的 isUIOnly 就是编译期间收集的那个属性。
        public async Task<string> JoinAsync(IStartupContext context, bool isUIOnly)
        {
            // 决定执行 Run 还是 RunAsync。
            // 进行性能统计，并报告结果。
            if (!isUIOnly)
            {
                return await Task.Run(async () =>
                {
                    try
                    {
                        await RunAsync(context);
                    }
                    finally
                    {
                        // 即使有异常，也是使用 SetResult 方法，因为启动项不会因为所依赖的启动项抛出异常而不执行
                        CompletedSource.SetResult(null);
                    }
                    return Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
                });
            }
            else
            {
                try
                {
                    await RunAsync(context);
                }
                finally
                {
                    // 即使有异常，也是使用 SetResult 方法，因为启动项不会因为所依赖的启动项抛出异常而不执行
                    CompletedSource.SetResult(null);
                }
                return Thread.CurrentThread.Name ??
                       Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 启动任务项的实际执行逻辑，由子类继承，实现启动任务项业务逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task RunAsync(IStartupContext context)
        {
            return CompletedTask;
        }

        /// <summary>
        /// 一个表示执行完成的任务，可以在 <see cref="RunAsync(IStartupContext)"/> 作为返回值
        /// </summary>
        protected internal static Task CompletedTask =>
#if NETFRAMEWORK
            CompletedCommonTask;
#pragma warning disable IDE0032 // Use auto property
        private static readonly Task CompletedCommonTask = Task.FromResult(true);
#pragma warning restore IDE0032 // Use auto property
#else
            Task.CompletedTask;
#endif

        /// <summary>
        /// 获取当前启动任务项可等待任务
        /// </summary>
        public Task TaskResult => CompletedSource.Task;

        private TaskCompletionSource<object?> CompletedSource { get; } = new TaskCompletionSource<object?>();

        internal IStartupManager Manager { get; set; }
        // 框架注入，一定不为空
            = null!;

        /// <summary>
        /// 获取某个指定传入的启动任务项的提供的参数
        /// </summary>
        /// <typeparam name="TStartup">提供参数的启动任务项</typeparam>
        /// <typeparam name="TValue">参数</typeparam>
        /// <returns></returns>
        protected TValue FetchValue<TStartup, TValue>() where TStartup : StartupTaskBase, IStartupValueProvider<TValue>
        {
            var task = Manager.GetStartupTask<TStartup>();
            var v = (IStartupValueProvider<TValue>) task;
            return v.ProvideValue();
        }
    }
}
