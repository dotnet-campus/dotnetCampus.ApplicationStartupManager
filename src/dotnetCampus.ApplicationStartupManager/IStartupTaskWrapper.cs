using System.Collections.Generic;

namespace dotnetCampus.ApplicationStartupManager
{
    public interface IStartupTaskWrapper
    {
        HashSet<string> FollowTasks { get; }
        HashSet<string> Dependencies { get; }
        string StartupTaskKey { get; }
        bool UIOnly { get; }
    }
}
