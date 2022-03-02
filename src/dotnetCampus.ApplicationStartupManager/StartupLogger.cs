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

        private readonly ConcurrentDictionary<string, (string name, long start, long elapsed)>
            _milestoneDictionary = new ConcurrentDictionary<string, (string, long, long)>();

        public StartupLoggerBase()
        {
            _mainWatch = new Stopwatch();
            _mainWatch.Start();
        }

        public void RecordTime(string milestoneName)
        {
            var start = _milestoneDictionary.Count > 0
                ? _milestoneDictionary.Max(x => x.Value.start + x.Value.elapsed)
                : 0;
            var end = _mainWatch.ElapsedTicks;
            _milestoneDictionary[milestoneName] =
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
                _milestoneDictionary[taskName] = (threadName, begin, elapse);
            }
        }

        public virtual void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers)
        {
        }
    }
}