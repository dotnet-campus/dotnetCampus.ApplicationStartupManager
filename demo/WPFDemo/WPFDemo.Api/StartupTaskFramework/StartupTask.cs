using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetCampus.ApplicationStartupManager;

namespace WPFDemo.Api.StartupTaskFramework
{
    /// <summary>
    /// 表示一个和当前业务强相关的启动任务
    /// </summary>
    public class StartupTask : StartupTaskBase
    {
        protected sealed override Task RunAsync(IStartupContext context)
        {
            return RunAsync((StartupContext) context);
        }

        protected virtual Task RunAsync(StartupContext context)
        {
            return CompletedTask;
        }
    }
}
