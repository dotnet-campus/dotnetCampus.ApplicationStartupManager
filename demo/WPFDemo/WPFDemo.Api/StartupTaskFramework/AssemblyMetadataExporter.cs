using System.Reflection;
using dotnetCampus.ApplicationStartupManager;
using dotnetCampus.Telescope;

namespace WPFDemo.Api.StartupTaskFramework
{
    public class AssemblyMetadataExporter
    {
        public AssemblyMetadataExporter(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<StartupTaskMetadata> ExportStartupTasks()
        {
            var collection = Export<StartupTask, StartupTaskAttribute>();
            return collection.Select(x => new StartupTaskMetadata(x.RealType.Name.Replace("Startup", ""), x.CreateInstance)
            {
                Scheduler = x.Attribute.Scheduler,
                BeforeTasks = x.Attribute.BeforeTasks,
                AfterTasks = x.Attribute.AfterTasks,
                //Categories = x.Attribute.Categories,
                CriticalLevel = x.Attribute.CriticalLevel,
            });
        }

        public IEnumerable<AttributedTypeMetadata<TBaseClassOrInterface, TAttribute>> Export<TBaseClassOrInterface, TAttribute>() where TAttribute : Attribute
        {
            return AttributedTypes.FromAssembly<TBaseClassOrInterface, TAttribute>(_assemblies);
        }

        private readonly Assembly[] _assemblies;
    }
}
