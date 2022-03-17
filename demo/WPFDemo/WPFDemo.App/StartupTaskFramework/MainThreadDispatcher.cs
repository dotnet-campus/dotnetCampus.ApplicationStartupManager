using System;
using System.Threading.Tasks;
using System.Windows;
using dotnetCampus.ApplicationStartupManager;

namespace WPFDemo.App.StartupTaskFramework
{
    // 因为没有在 WPFDemo.Api 引用 WPF 程序集，因此代码写在这里
    class MainThreadDispatcher : IMainThreadDispatcher
    {
        public async Task InvokeAsync(Action action)
        {
            await Application.Current.Dispatcher.InvokeAsync(action);
        }
    }
}
