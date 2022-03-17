using System;

namespace dotnetCampus.ApplicationStartupManager
{
    public class StartupTaskBuilder
    {
        private readonly StartupTaskWrapper _wrapper;
        private readonly Action<StartupTaskWrapper, string> _addDependenciesAction;
        private readonly Action<StartupTaskWrapper, string> _addFollowTasksAction;

        internal StartupTaskBuilder(StartupTaskWrapper wrapper,
            Action<StartupTaskWrapper, string> addDependenciesAction,
            Action<StartupTaskWrapper, string> addFollowTasksAction)
        {
            _wrapper = wrapper;
            _addDependenciesAction = addDependenciesAction;
            _addFollowTasksAction = addFollowTasksAction;
        }

        public StartupCategory Categories
        {
            get => _wrapper.Categories;
            set => _wrapper.Categories = value;
        }

        public StartupTaskBuilder AddDependencies(string afterTasks)
        {
            _addDependenciesAction(_wrapper, afterTasks);
            return this;
        }

        public StartupTaskBuilder AddFollowTasks(string beforeTasks)
        {
            _addFollowTasksAction(_wrapper, beforeTasks);
            return this;
        }
    }
}
