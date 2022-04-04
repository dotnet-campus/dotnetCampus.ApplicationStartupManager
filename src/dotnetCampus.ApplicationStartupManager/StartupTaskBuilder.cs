using System;

namespace dotnetCampus.ApplicationStartupManager
{
    /// <summary>
    /// 启动流程任务创建器
    /// </summary>
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

        /// <summary>
        /// 设置或获取启动流程的分类
        /// </summary>
        public StartupCategory Categories
        {
            get => _wrapper.Categories;
            set => _wrapper.Categories = value;
        }

        /// <summary>
        /// 加上依赖的启动任务项
        /// </summary>
        /// <param name="afterTasks"></param>
        /// <returns></returns>
        public StartupTaskBuilder AddDependencies(string afterTasks)
        {
            _addDependenciesAction(_wrapper, afterTasks);
            return this;
        }

        /// <summary>
        /// 加上跟随当前启动任务项的启动任务项
        /// </summary>
        /// <param name="beforeTasks"></param>
        /// <returns></returns>
        public StartupTaskBuilder AddFollowTasks(string beforeTasks)
        {
            _addFollowTasksAction(_wrapper, beforeTasks);
            return this;
        }
    }
}
