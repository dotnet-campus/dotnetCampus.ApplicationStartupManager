using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    public abstract class StartupTaskBase
    {
        // 由于我们都在编译期间收集 Attribute 了，当然也能收集使用方到底重写了哪个 Run。
        // 这里传入的 isUIOnly 就是编译期间收集的那个属性。
        public async Task<string> JoinAsync(IStartupContext context, bool isUIOnly)
        {
            // 决定执行 Run 还是 RunAsync。
            // 进行性能统计，并报告结果。
            if (!isUIOnly)
            {
                return await Task.Run(async () =>
                {
                    await RunAsync(context);
                    CompletedSource.SetResult(null);
                    return Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
                });
            }
            else
            {
                await RunAsync(context);
                CompletedSource.SetResult(null);
                return Thread.CurrentThread.Name ??
                       Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
            }
        }

        protected virtual Task RunAsync(IStartupContext context)
        {
            return CompletedTask;
        }

        protected internal static Task CompletedTask =>
#if NETFRAMEWORK
            CompletedCommonTask;
        private static readonly Task CompletedCommonTask = Task.FromResult(true);
#else
            Task.CompletedTask;
#endif

        public Task TaskResult => CompletedSource.Task;

        private TaskCompletionSource<object> CompletedSource { get; } = new TaskCompletionSource<object>();

        internal IStartupManager Manager { get; set; }

        protected TValue FetchValue<TStartup, TValue>() where TStartup : StartupTaskBase, IStartupValueProvider<TValue>
        {
            var task = Manager.GetStartupTask<TStartup>();
            var v = (IStartupValueProvider<TValue>)task;
            return v.ProvideValue();
        }
    }
}