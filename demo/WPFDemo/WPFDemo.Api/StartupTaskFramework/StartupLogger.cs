using System.Diagnostics;
using System.Text;

using dotnetCampus.ApplicationStartupManager;

namespace WPFDemo.Api.StartupTaskFramework
{
    /// <summary>
    /// 和项目关联的日志
    /// </summary>
    public class StartupLogger : StartupLoggerBase
    {
        public void LogInfo(string message)
        {
            Debug.WriteLine(message);
        }

        public override void ReportResult(IReadOnlyList<IStartupTaskWrapper> wrappers)
        {
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in MilestoneDictionary)
            {
                stringBuilder.AppendLine($"{keyValuePair.Key} - [{keyValuePair.Value.threadName}] Start:{keyValuePair.Value.start} Elapsed:{keyValuePair.Value.elapsed}");
            }

            Debug.WriteLine(stringBuilder.ToString());
        }
    }
}
