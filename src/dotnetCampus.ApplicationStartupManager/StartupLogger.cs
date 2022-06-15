using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 表示启动任务的基础日志，推荐业务方提供更贴合业务的日志，和日志记录的方法
    /// </summary>
    public class StartupLoggerBase : IStartupLogger
    {
        private readonly Stopwatch _mainWatch;

        /// <summary>
        /// 各个启动的里程碑的信息，包括里程碑的所运行的线程名，启动时间和执行时间
        /// <para>
        /// Key: 启动的里程碑名
        /// </para>
        /// <para>
        /// Value: 启动的信息，包括里程碑的所运行的线程名，启动时间和执行时间。时间都是从 <see cref="Stopwatch.ElapsedTicks"/> 获取
        /// </para>
        /// </summary>
        protected ConcurrentDictionary<string, (string threadName, long start, long elapsed)>
            MilestoneDictionary
        { get; } = new ConcurrentDictionary<string, (string, long, long)>();

        /// <summary>
        /// 创建启动任务的基础日志，此日志的核心功能就是监控启动项的启动时间
        /// </summary>
        public StartupLoggerBase()
        {
            _mainWatch = new Stopwatch();
            _mainWatch.Start();
        }

        /// <inheritdoc/>
        public void RecordTime(string milestoneName)
        {
            var start = MilestoneDictionary.Count > 0
                ? MilestoneDictionary.Max(x => x.Value.start + x.Value.elapsed)
                : 0;
            var end = _mainWatch.ElapsedTicks;
            MilestoneDictionary[milestoneName] =
                (Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture),
                    start, end - start);
        }

        /// <inheritdoc/>
        public async Task RecordDuration(string taskName, Func<Task<string>> task)
        {
            var threadName = "null";
            var begin = _mainWatch.ElapsedTicks;

            try
            {
                threadName = await task().ConfigureAwait(false);
            }
            finally
            {
                var end = _mainWatch.ElapsedTicks;
                var elapse = end - begin;
                MilestoneDictionary[taskName] = (threadName, begin, elapse);
            }
        }

        /// <inheritdoc/>
        public virtual void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers)
        {
            // 没有实际的可以记录的地方，需要业务方自己实现记录到哪
        }
    }
}
