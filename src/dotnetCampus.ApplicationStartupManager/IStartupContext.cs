using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 表示启动任务的上下文接口
    /// </summary>
    public interface IStartupContext
    {
        /// <summary>
        /// 等待某个启动任务完成
        /// </summary>
        /// <param name="startupKey"></param>
        /// <returns></returns>
        /// 这是框架层需要支持的能力，因此也就放在此
        Task WaitStartupTaskAsync(string startupKey);
    }
}
