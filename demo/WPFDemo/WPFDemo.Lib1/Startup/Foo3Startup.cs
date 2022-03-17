using dotnetCampus.ApplicationStartupManager;

using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.Lib1.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.CoreUI, AfterTaskList = new[] { nameof(WPFDemo.Lib1.Startup.Foo2Startup), "Foo1Startup" })]
    public class Foo3Startup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            context.Logger.LogInfo("Foo3 Startup");
            return base.RunAsync(context);
        }
    }
}
