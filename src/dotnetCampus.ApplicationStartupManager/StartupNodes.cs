namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 包含预设的启动节点。
    /// </summary>
    public class StartupNodes
    {
        /// <summary>
        /// 基础服务（日志、异常处理、容器、生命周期管理等）请在此节点之前启动，其他业务请在此之后启动。
        /// </summary>
        public const string Foundation = "Foundation";

        /// <summary>
        /// 需要在任何一个 界面 创建之前启动的任务请在此节点之前。
        /// 此节点之后将开始启动 UI。
        /// </summary>
        public const string CoreUI = "CoreUI";

        /// <summary>
        /// 需要在主 界面 创建之后启动的任务请在此节点之后。
        /// 此节点完成则代表主要 UI 已经初始化完毕（但不一定已显示）。
        /// </summary>
        public const string UI = "UI";

        /// <summary>
        /// 主 界面 已布局、渲染完毕，对用户完全可见，可开始交互的时机。
        /// 不被其他业务依赖的模块可在此节点之后启动。
        /// </summary>
        public const string MainWindow = "MainWindow";

        /// <summary>
        /// 任何不关心何时启动的启动任务应该设定为在此节点之前完成。
        /// </summary>
        public const string StartupCompleted = "StartupCompleted";
    }
}