using System.ComponentModel;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 表示启动任务的关键级别，用于在启动流程执行的过程中评估错误的影响范围。
    /// </summary>
    public enum StartupCriticalLevel
    {
        /// <summary>
        /// 表示没有评估过此模块的关键级别。
        /// 如果你无法评估此模块的关键级别，请保持默认值。
        /// </summary>
        Unset = 0,

        /// <summary>
        /// 表示这是一个扩展的启动任务，如果此任务初始化失败，软件不会有任何业务功能受到影响。
        /// 通常插件、性能和数据监控等模块会使用此级别。
        /// </summary>
        Extension,

        /// <summary>
        /// 表示这是一个普通的启动任务，如果此任务初始化失败，软件的多数功能不会受到影响。
        /// 通常如果一个启动任务初始化的模块不被其他任何模块使用到，那么指定为此级别。
        /// </summary>
        Normal,

        /// <summary>
        /// 表示这是一个关键启动任务，如果此任务初始化失败，软件将很难正常运行
        /// </summary>
        Critical,
    }
}
