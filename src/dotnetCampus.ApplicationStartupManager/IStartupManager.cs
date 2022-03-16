using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupManager
    {
        Task WaitStartupTaskAsync(string startupTaskKey);

        StartupTaskBase GetStartupTask<T>() where T : StartupTaskBase;
    }
}