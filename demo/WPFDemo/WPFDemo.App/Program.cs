using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using dotnetCampus.Cli;
using dotnetCampus.Configurations.Core;

using WPFDemo.Api.Startup;
using WPFDemo.Api.StartupTaskFramework;
using WPFDemo.App.StartupTaskFramework;
using WPFDemo.Lib1.Startup;

namespace WPFDemo.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var commandLine = CommandLine.Parse(args);

            var app = new App();

            //开始启动任务
            StartStartupTasks(commandLine);

            app.Run();
        }

        private static void StartStartupTasks(CommandLine commandLine)
        {
            Task.Run(() =>
            {
                // 获取应用配置文件逻辑
                var configFilePath = "App.coin";
                var repo = ConfigurationFactory.FromFile(configFilePath);

                var assemblyMetadataExporter = new AssemblyMetadataExporter(BuildStartupAssemblies());

                var startupManager = new StartupManager(commandLine, repo, HandleShutdownError, new MainThreadDispatcher())
                    .UseCriticalNodes
                    (
                        StartupNodes.Foundation,
                        StartupNodes.CoreUI,
                        StartupNodes.UI,
                        StartupNodes.AppReady,
                        StartupNodes.StartupCompleted
                    )
                    // 导出程序集的启动项
                    .AddStartupTaskMetadataCollector(() =>
                        assemblyMetadataExporter.ExportStartupTasks());
                startupManager.Run();
            });
        }

        private static Assembly[] BuildStartupAssemblies()
        {
            // 初始化预编译收集的所有模块。
            return new Assembly[]
            {
                // WPFDemo.App
                typeof(Program).Assembly,
                // WPFDemo.Lib1
                typeof(Foo2Startup).Assembly,
                // WPFDemo.Api
                typeof(Foo1Startup).Assembly,
            };
        }

        private static Task HandleShutdownError(Exception ex)
        {
            // 这是启动过程的异常，需要进行退出

#if DEBUG
            Debug.WriteLine("========== [初始化过程中出现致命错误，详情请查看异常信息] ==========");
            Debug.WriteLine(ex.ToString());

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
#endif

            return Task.CompletedTask;
        }
    }
}
