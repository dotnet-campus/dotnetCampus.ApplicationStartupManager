using dotnetCampus.Cli;

namespace WPFDemo.Api.CommandLines
{
    /// <summary>
    /// 启动参数
    /// </summary>
    public class Options
    {
        [Option("Name")]
        public string? Name { set; get; }
    }
}
