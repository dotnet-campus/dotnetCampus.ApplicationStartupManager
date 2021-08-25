namespace dotnetCampus.ApplicationStartupManager
{
    static class StartupTaskHelper
    {
        public static string BuildTasks(params string[] taskList)
        {
            if (taskList.Length > 1)
            {
                return string.Join(";", taskList);
            }

            if (taskList.Length == 1)
            {
                return taskList[0];
            }

            return string.Empty;
        }
    }
}