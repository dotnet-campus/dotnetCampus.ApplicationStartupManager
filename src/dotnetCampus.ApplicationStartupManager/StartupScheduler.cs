namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 表示启动任务将如何安排执行。
    /// </summary>
    public enum StartupScheduler
    {
        /// <summary>
        /// 允许在所有线程执行。
        /// </summary>
        Default,

        /// <summary>
        /// 要求必须在主 UI 线程执行。
        /// </summary>
        UIOnly,
    }
}
