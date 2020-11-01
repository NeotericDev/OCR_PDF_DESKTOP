using Akka.Actor;
using OCR_PDF.Messages;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCR_PDF.ViewModels
{
    public class MainContentControlViewModel : BindableBase
    {
        public delegate void TaskCountHandler(Int32 taskCount);
        public event TaskCountHandler TaskCountUpdated;

        private string bg = "Black";
        public string BG
        {
            get { return bg = "Black"; }
            set { SetProperty(ref bg, value); }
        }

        private ObservableCollection<TaskViewModel> tasks;
        public ObservableCollection<TaskViewModel> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }

        private string outputFolder;
        public string OutputFolder
        {
            get { return outputFolder; }
            set { SetProperty(ref outputFolder, value); }
        }

        public MainContentControlViewModel()
        {
                       
        }

        private DelegateCommand addTask;
        public DelegateCommand AddTask =>
            addTask ?? (addTask = new DelegateCommand(ExecuteAddTask));

        void ExecuteAddTask()
        {
            AddNewTask();
        }

        private DelegateCommand selectFolderCmd;
        public DelegateCommand SelectFolderCmd =>
            selectFolderCmd ?? (selectFolderCmd = new DelegateCommand(ExecuteSelectFolderCmd));

        void ExecuteSelectFolderCmd()
        {
            // TODO : Add Folder Browser Code
        }


        public void AddNewTask()
        {
            if(Tasks == null)
            {
                Tasks = new ObservableCollection<TaskViewModel>();
            }
            Tasks.Add(new TaskViewModel());
            NotifyTaskCount();
        }

        internal void RemoveTask(TaskViewModel taskToRemove)
        {
            if(Tasks != null)
            {
                Tasks.Remove(taskToRemove);
                NotifyTaskCount();
            }
        }

        private void NotifyTaskCount()
        {
            TaskCountUpdated?.Invoke(Tasks.Count);
        }

        public void StartProcessing()
        {
            App.UiActor.Tell(new MsgStartProcessing(Guid.NewGuid(), Tasks, null, OutputFolder), ActorRefs.NoSender);
        }
    }
}
