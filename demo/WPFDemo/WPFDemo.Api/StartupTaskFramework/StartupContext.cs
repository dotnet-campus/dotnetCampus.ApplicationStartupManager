using dotnetCampus.ApplicationStartupManager;
using dotnetCampus.Cli;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using WPFDemo.Api.CommandLines;

namespace WPFDemo.Api.StartupTaskFramework
{
    public class StartupContext : IStartupContext
    {
        public StartupContext(IStartupContext startupContext, CommandLine commandLine, StartupLogger logger, FileConfigurationRepo configuration, IAppConfigurator configs)
        {
            _startupContext = startupContext;
            Logger = logger;
            Configuration = configuration;
            Configs = configs;
            CommandLine = commandLine;
            CommandLineOptions = CommandLine.As<Options>();
        }

        public StartupLogger Logger { get; }

        public CommandLine CommandLine { get; }

        public Options CommandLineOptions { get; }

        public FileConfigurationRepo Configuration { get; }

        public IAppConfigurator Configs { get; }

        public Task<string> ReadCacheAsync(string key, string @default = "")
        {
            return Configuration.TryReadAsync(key, @default);
        }

        private readonly IStartupContext _startupContext;
        public Task WaitStartupTaskAsync(string startupKey)
        {
            return _startupContext.WaitStartupTaskAsync(startupKey);
        }
    }
}
