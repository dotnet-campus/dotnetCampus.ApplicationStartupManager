using dotnetCampus.ApplicationStartupManager;
using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.Api.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.Foundation, AfterTasks = "LibStartup")]
    class OptionStartup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            context.Logger.LogInfo("Command " + context.CommandLineOptions.Name);

            return CompletedTask;
        }
    }
}
