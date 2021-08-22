using System;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 主线程执行调度
    /// </summary>
    public interface IMainThreadDispatcher
    {
        /// <summary>
        /// 调度执行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task InvokeAsync(Action action);
    }
}