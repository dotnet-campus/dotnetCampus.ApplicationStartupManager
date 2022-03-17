using System.Threading.Tasks;
//using dotnetCampus.Configurations;

namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupContext
    {
        //IAppConfigurator Configs { get; }
        //Task<string> ReadCacheAsync(string key, string @default = "");
        Task WaitStartupTaskAsync(string startupKey);
    }
}
