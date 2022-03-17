using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using dotnetCampus.ApplicationStartupManager;
using WPFDemo.Api.StartupTaskFramework;

namespace WPFDemo.App.Startup
{
    [StartupTask(BeforeTasks = StartupNodes.AppReady, AfterTasks = "MainWindowStartup", Scheduler = StartupScheduler.UIOnly)]
    internal class BusinessStartup : StartupTask
    {
        protected override Task RunAsync(StartupContext context)
        {
            if (Application.Current.MainWindow.Content is Grid grid)
            {
                grid.Children.Add(new Button()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(10, 10, 10, 10),
                    Content = "Click"
                });
            }

            return CompletedTask;
        }
    }
}
