using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupManager
    {
        Task WaitStartupTaskAsync(string startupTaskKey);

        StartupTask GetStartupTask<T>() where T : StartupTask;
    }
}