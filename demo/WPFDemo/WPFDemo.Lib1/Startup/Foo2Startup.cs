using dotnetCampus.ApplicationStartupManager;

using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.Lib1.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.CoreUI, AfterTasks = StartupNodes.Foundation)]
    public class Foo2Startup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            context.Logger.LogInfo("Foo2 Startup");
            return base.RunAsync(context);
        }
    }
}
