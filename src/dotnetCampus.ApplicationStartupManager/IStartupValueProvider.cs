namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupValueProvider<out T>
    {
        T ProvideValue();
    }
}
