using System;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动任务的特性，可以让业务方用来对接，如对接预编译框架，从而收集启动任务项
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class StartupTaskAttribute : Attribute
    {
        /// <summary>
        /// 此启动任务的任务必须在指定的其他启动任务之前完成。可以使用 “;” 分隔符指定多个启动任务。
        /// </summary>
        public string? BeforeTasks { get; set; }

        /// <summary>
        /// 效果上等同于 <see cref="BeforeTasks"/> 属性，此属性只是为了开发方便而已
        /// </summary>
        public string[]? BeforeTaskList
        {
            set
            {
                BeforeTasks = StartupTaskHelper.BuildTasks(value);
            }
            get
            {
                return BeforeTasks?.Split(';');
            }
        }

        /// <summary>
        /// 此启动任务的任务将在指定的其他启动任务之后开始执行。可以使用 “;” 分隔符指定多个启动任务。
        /// </summary>
        public string? AfterTasks { get; set; }

        /// <summary>
        /// 效果上等同于 <see cref="AfterTasks"/> 属性，此属性只是为了开发方便而已
        /// </summary>
        public string[]? AfterTaskList
        {
            set
            {
                AfterTasks = StartupTaskHelper.BuildTasks(value);
            }
            get
            {
                return AfterTasks?.Split(';');
            }
        }

        /// <summary>
        /// 指定启动任务任务的上下文。
        /// </summary>
        public StartupScheduler Scheduler { get; set; } = StartupScheduler.Default;

        /// <summary>
        /// 获取或设置此启动任务的关键级别。
        /// </summary>
        public StartupCriticalLevel CriticalLevel { get; set; }
    }
}
