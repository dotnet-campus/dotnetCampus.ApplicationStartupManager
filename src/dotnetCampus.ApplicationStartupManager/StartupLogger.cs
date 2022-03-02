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
    public class StartupLoggerBase : IStartupLogger
    {
        private readonly Stopwatch _mainWatch;

        protected ConcurrentDictionary<string, (string threadName, long start, long elapsed)>
            MilestoneDictionary { get; } = new ConcurrentDictionary<string, (string, long, long)>();

        public StartupLoggerBase()
        {
            _mainWatch = new Stopwatch();
            _mainWatch.Start();
        }

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

        public virtual void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers)
        {
        }
    }
}