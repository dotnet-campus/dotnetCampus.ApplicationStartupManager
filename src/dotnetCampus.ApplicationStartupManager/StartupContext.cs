using System;
using System.Threading.Tasks;
//using dotnetCampus.Configurations;
//using dotnetCampus.Configurations.Core;

namespace dotnetCampus.ApplicationStartupManager
{
    internal class StartupContext : IStartupContext
    {
        public IStartupLogger Logger { get; }

        //public FileConfigurationRepo Configuration { get; }

        //public IAppConfigurator Configs { get; }

        public Func<Exception, Task> FastFail { get; }

        private readonly Func<string, Task> _waitStartupTaskAsync;

        //public Task<string> ReadCacheAsync(string key, string @default = "")
        //{
        //    return Configuration.TryReadAsync(key, @default);
        //}

        Task IStartupContext.WaitStartupTaskAsync(string startupKey) => _waitStartupTaskAsync(startupKey);

        public StartupContext(IStartupLogger logger, /*FileConfigurationRepo configuration,*/
            Func<Exception, Task> fastFailAction, Func<string, Task> waitStartupAsync)
        {
            Logger = logger;
            //Configuration = configuration;
            _waitStartupTaskAsync = waitStartupAsync;
            //Configs = configuration.CreateAppConfigurator();
            FastFail = fastFailAction ?? (exception => StartupTaskBase.CompletedTask);
        }
    }
}
