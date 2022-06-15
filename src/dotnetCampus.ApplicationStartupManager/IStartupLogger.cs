using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动模块的日志
    /// </summary>
    public interface IStartupLogger
    {
        /// <summary>
        /// 打点某个里程碑，将会自动与上个里程碑记录时间差
        /// </summary>
        /// <param name="milestoneName"></param>
        void RecordTime(string milestoneName);
        /// <summary>
        /// 记录某个任务的耗时情况
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        Task RecordDuration(string taskName, Func<Task<string>> task);
        /// <summary>
        /// 上报启动结果
        /// </summary>
        /// <param name="wrappers"></param>
        void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers);
    }
}
