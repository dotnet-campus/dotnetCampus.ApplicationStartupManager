using System.Collections.Generic;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动项在启动框架的信息
    /// </summary>
    public interface IStartupTaskWrapper
    {
        /// <summary>
        /// 跟随等待着当前启动项的其他启动项
        /// </summary>
        HashSet<string> FollowTasks { get; }
        /// <summary>
        /// 当前启动项所依赖的其他启动项
        /// </summary>
        HashSet<string> Dependencies { get; }
        /// <summary>
        /// 当前启动项的标识
        /// </summary>
        string StartupTaskKey { get; }
        /// <summary>
        /// 表示当前启动项是否只能在 UI 线程启动
        /// </summary>
        bool UIOnly { get; }
    }
}
