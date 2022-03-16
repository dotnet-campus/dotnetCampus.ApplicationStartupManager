using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.ApplicationStartupManager;

using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.App.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.AppReady, AfterTasks = StartupNodes.UI, Scheduler = StartupScheduler.UIOnly)]
    internal class MainWindowStartup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            return CompletedTask;
        }
    }
}
