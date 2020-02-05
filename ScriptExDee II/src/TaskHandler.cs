using System;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace ScriptExDee_II
{
    public static class TaskHandler
    {
        private const string taskDef = "ScriptExDee";

        // Code adapted from Magnus Hjort, Builder Companion
        /// <summary>
        /// Creates a Windows Task which will start up this application with admin rights at every startup.
        /// </summary>
        public static void CreateTaskService()
        {
            TaskService ts = new TaskService();
            TaskDefinition td = ts.NewTask();
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Triggers.AddNew(TaskTriggerType.Logon);
            //td.Triggers.AddNew(TaskTriggerType.Once);   
            string program_path = Path.Combine(Program.BasePath);

            td.Actions.Add(new ExecAction(program_path, null));
            ts.RootFolder.RegisterTaskDefinition(taskDef, td);

        }

        /// <summary>
        /// Deletes the Windows Task for this application if any.
        /// </summary>
        public static void DeleteTaskService()
        {
            EnumAllTasks();
        }

        private static void EnumAllTasks()
        {
            EnumFolderTasks(TaskService.Instance.RootFolder);
        }

        private static void EnumFolderTasks(TaskFolder fld)
        {
            foreach (Task t in fld.Tasks)
                DeleteTask(t);
            foreach (TaskFolder sfld in fld.SubFolders)
                EnumFolderTasks(sfld);
        }

        private static void DeleteTask(Task t)
        {
            // Only delete if our task
            if (t.Name.Equals(taskDef))
            {
                TaskService ts = t.TaskService;
                ts.RootFolder.DeleteTask(t.Name);
            }
        }


        public static bool TaskExists = false;
        /// <summary>
        /// Check to see if service exists
        /// </summary>
        public static void CheckTaskService()
        {
            TaskExists = false;
            CheckAllTasks();
        }

        private static void CheckAllTasks()
        {
            CheckFolderTasks(TaskService.Instance.RootFolder);
        }

        private static void CheckFolderTasks(TaskFolder fld)
        {
            foreach (Task t in fld.Tasks)
                CheckTask(t);
            foreach (TaskFolder sfld in fld.SubFolders)
                CheckFolderTasks(sfld);
        }

        private static void CheckTask(Task t)
        {
            // Only delete if our task
            if (t.Name.Equals(taskDef))
            {
                TaskExists = true;
            }
        }
    }
}
