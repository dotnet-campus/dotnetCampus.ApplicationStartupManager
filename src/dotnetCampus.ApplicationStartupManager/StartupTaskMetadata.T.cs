using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 记录 <typeparamref name="TStartupTask"/> 类型中标记的从 <see cref="StartupTaskAttribute"/> 中统一收集元数据。
    /// </summary>
    /// <typeparam name="TStartupTask"></typeparam>
    public class StartupTaskMetadata<TStartupTask> : StartupTaskMetadata where TStartupTask : StartupTaskBase, new()
    {
        /// <summary>
        /// 创建包含 <typeparamref name="TStartupTask"/> 元数据的 <see cref="StartupTaskMetadata{TStartupTask}"/> 的新实例。
        /// </summary>
        public StartupTaskMetadata() : base(typeof(TStartupTask).Name.Replace("Startup", ""), () => new TStartupTask())
        {
        }
    }
}
