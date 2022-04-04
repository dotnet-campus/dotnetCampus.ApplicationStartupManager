namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 用于启动项里的值提供器，用于多个启动项之间传递参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStartupValueProvider<out T>
    {
        /// <summary>
        /// 获取启动项提供的值
        /// </summary>
        /// <returns></returns>
        T ProvideValue();
    }
}
