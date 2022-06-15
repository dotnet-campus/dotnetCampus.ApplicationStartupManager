using System.Threading.Tasks;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动流程管理器
    /// </summary>
    public interface IStartupManager
    {
        /// <summary>
        /// 等待某个启动项的完成
        /// </summary>
        /// <param name="startupTaskKey"></param>
        /// <returns></returns>
        Task WaitStartupTaskAsync(string startupTaskKey);

        /// <summary>
        /// 获取指定类型的启动项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// 理论上相同的类型的启动项不会被重复加入，基本都是一个启动项一个类型
        StartupTaskBase GetStartupTask<T>() where T : StartupTaskBase;
    }
}
