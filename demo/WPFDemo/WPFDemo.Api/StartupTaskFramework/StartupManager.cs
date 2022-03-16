using dotnetCampus.ApplicationStartupManager;
using dotnetCampus.Cli;
using dotnetCampus.Configurations.Core;

namespace WPFDemo.Api.StartupTaskFramework
{
    /// <summary>
    /// 和项目关联的启动管理器，用来注入业务相关的逻辑
    /// </summary>
    public class StartupManager : StartupManagerBase
    {
        public StartupManager(CommandLine commandLine, FileConfigurationRepo configuration, Func<Exception, Task> fastFailAction, IMainThreadDispatcher mainThreadDispatcher) : base(new StartupLogger(), fastFailAction, mainThreadDispatcher)
        {
            var appConfigurator = configuration.CreateAppConfigurator();
            Context = new StartupContext(StartupContext, commandLine, (StartupLogger) Logger, configuration, appConfigurator);
        }

        private StartupContext Context { get; }

        protected override Task<string> ExecuteStartupTaskAsync(StartupTaskBase startupTask, IStartupContext context, bool uiOnly)
        {
            return base.ExecuteStartupTaskAsync(startupTask, Context, uiOnly);
        }
    }
}
