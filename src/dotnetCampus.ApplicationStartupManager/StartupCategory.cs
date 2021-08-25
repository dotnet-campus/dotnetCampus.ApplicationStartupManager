using System;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 表示启动流程的分类
    /// </summary>
    [Flags]
    public enum StartupCategory
    {
        /// <summary>
        /// 用于启动流程快速上下架
        /// </summary>
        None = 0,

        /// <summary>
        /// 用于所有模式都启动
        /// </summary>
        All = 1,
    }
}