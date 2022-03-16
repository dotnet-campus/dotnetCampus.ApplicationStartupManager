using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.ApplicationStartupManager;

using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.Api.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.CoreUI, AfterTasks = StartupNodes.Foundation)]
    public class Foo1Startup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            context.Logger.LogInfo("Foo1 Startup");
            return base.RunAsync(context);
        }
    }
}
