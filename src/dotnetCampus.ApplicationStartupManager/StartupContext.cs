using System;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动项的上下文信息，业务方可以自己再定义而不需要使用此类型
    /// </summary>
    internal class StartupContext : IStartupContext
    {
        public IStartupLogger Logger { get; }

        public Func<Exception, Task> FastFail { get; }

        private readonly Func<string, Task> _waitStartupTaskAsync;

        Task IStartupContext.WaitStartupTaskAsync(string startupKey) => _waitStartupTaskAsync(startupKey);

        public StartupContext(IStartupLogger logger, /*FileConfigurationRepo configuration,*/
            Func<Exception, Task> fastFailAction, Func<string, Task> waitStartupAsync)
        {
            Logger = logger;
            _waitStartupTaskAsync = waitStartupAsync;
            FastFail = fastFailAction ?? (exception => StartupTaskBase.CompletedTask);
        }
    }
}
