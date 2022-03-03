using System;
using System.Threading;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 记录 <see cref="StartupTaskBase"/> 类型中标记的从 <see cref="StartupTaskAttribute"/> 中统一收集元数据。
    /// </summary>
    public class StartupTaskMetadata
    {
        private readonly Lazy<StartupTaskBase> _taskLazy;

        /// <summary>
        /// 创建 <see cref="StartupTaskMetadata"/> 的新实例。
        /// </summary>
        /// <param name="key">表示此 <see cref="StartupTaskBase"/> 的唯一标识符。</param>
        /// <param name="creator">此 <see cref="StartupTaskBase"/> 实例的创建方法。</param>
        public StartupTaskMetadata( string key, Func<StartupTaskBase> creator)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _taskLazy = new Lazy<StartupTaskBase>(
                creator ?? throw new ArgumentNullException(nameof(creator)),
                LazyThreadSafetyMode.None);
        }

        /// <summary>
        /// 获取 <see cref="StartupTaskBase"/> 的唯一标识符。
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 获取 Categories 的值
        /// </summary>
        public StartupCategory Categories { get; set; } = StartupCategory.All;

        /// <summary>
        /// 获取 <see cref="StartupTaskAttribute.BeforeTasks"/> 的值，如果没有标记，则为 null。
        /// </summary>
        public string? BeforeTasks { get; set; }

        /// <summary>
        /// 获取 <see cref="StartupTaskAttribute.AfterTasks"/> 的值，如果没有标记，则为 null。
        /// </summary>
        public string? AfterTasks { get; set; }

        /// <summary>
        /// 获取 <see cref="StartupTaskAttribute.Scheduler"/> 的值，如果没有标记，则为 <see cref="StartupScheduler.Default"/>。
        /// </summary>
        public StartupScheduler Scheduler { get; set; }

        /// <summary>
        /// 根据从元数据中收集到的创建 <see cref="StartupTaskBase"/> 的方法获取或创建 <see cref="StartupTaskBase"/> 的实例。
        /// </summary>
        public StartupTaskBase Instance => _taskLazy.Value;

        /// <summary>
        /// 获取 <see cref="StartupTaskAttribute.CriticalLevel"/> 的值，如果没有获取或设置此启动任务的关键级别，则为 <see cref="StartupCriticalLevel.Unset"/>。
        /// </summary>
        public StartupCriticalLevel CriticalLevel { get; set; }
    }
}