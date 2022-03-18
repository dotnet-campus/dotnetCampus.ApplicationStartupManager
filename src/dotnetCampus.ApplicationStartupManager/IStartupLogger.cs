using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupLogger
    {
        void RecordTime(string milestoneName);
        Task RecordDuration(string taskName, Func<Task<string>> task);
        void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers);
    }
}
