using dotnetCampus.ApplicationStartupManager;
using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.Lib1.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.Foundation)]
    class LibStartup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            context.Logger.LogInfo("Lib Startup");
            return base.RunAsync(context);
        }
    }
}
