using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using OCR_PDF.Actors.Workers;
using OCR_PDF.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCR_PDF.Actors.UI
{
    public class TaskDispatcher : CancellableStashingActor
    {
        readonly IActorRef Worker;
        readonly IActorRef ProgressUpdater;
        public TaskDispatcher() : base()
        {
            ProgressUpdater = Context.ActorOf(Context.DI().Props<ProgressUpdater>().WithRouter(FromConfig.Instance), "ProgressUpdaterActor");
            Worker = Context.ActorOf(Context.DI().Props<TaskActor>().WithRouter(FromConfig.Instance), "TaskActor");
            System.Diagnostics.Debug.WriteLine(Self.Path + " |=| UiCoordinatorActor :: Dispatcher In Use : " + Context.Dispatcher.Id + " | " + Context.Dispatcher.GetType().Name + " | " + Context.Dispatcher.ToString());
            ReadyToStartWork();
        }

        public void ReadyToStartWork() 
        {
            Receive<MsgStartProcessing>((msg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgStartProcessing(ReadyToStartWork) : " + msg.Id);
                ProcessAsync(msg.Id, ()=>
                {
                    Dictionary<Guid, FileInfo> fileInfoDict = new Dictionary<Guid, FileInfo>();
                    if (ValidateInputTasks(msg, fileInfoDict))
                    {
                        MsgBeReadyToGetUpdates msgBeReadyToGetUpdates = new MsgBeReadyToGetUpdates(msg.Id, msg.Tasks);
                        ProgressUpdater.Tell(new Broadcast(msgBeReadyToGetUpdates));
                        SplitAndAssignTasksToWorkers(msg, fileInfoDict);
                    }

                }, (continuedTask) =>
                {
                    UpdateStatusOnFailure(msg, continuedTask);
                });
            });
            Receive<MsgCancelAllTasks>((msg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelAllTasks(ReadyToStartWork) : " + msg.Id);
                CancelToken(msg.Id);
                Worker.Tell(new Broadcast(msg));
                ProgressUpdater.Tell(msg);
            });
            Receive<MsgCancelTask>((cancelTask) =>
            {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTask(ReadyToStartWork) :: TaskId :: " + cancelTask.TaskId);
                Worker.Tell(new Broadcast(cancelTask));
                ProgressUpdater.Tell(cancelTask);
            });
            Receive<MsgCancelTaskUnit>((cancelTask) =>
            {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTaskUnit(ReadyToStartWork) :: TaskId :: " + cancelTask.TaskId);
                Worker.Tell(new Broadcast(cancelTask));
                ProgressUpdater.Tell(cancelTask);
            });
        }

        private static void UpdateStatusOnFailure(MsgStartProcessing msg, Task continuedTask)
        {
            string status = string.Empty;
            bool isInterrupted = false;
            if (continuedTask.IsCanceled)
            {
                status = "Operation Cancelled By User..";
                isInterrupted = true;                
            }
            else if (continuedTask.IsFaulted)
            {
                status = "Some fault occured while processing..";
                isInterrupted = true;
            }
            if (isInterrupted)
            {
                foreach (var taskVM in msg.Tasks)
                {
                    if (!taskVM.InProgress)
                    {
                        taskVM.InProgress = false;
                        taskVM.Status = "Operation Cancelled By User..";
                    }
                }
            }
        }

        private void SplitAndAssignTasksToWorkers(MsgStartProcessing msg, Dictionary<Guid, FileInfo> fileInfoDict)
        {
            msg.Tasks.ToList().ForEach((taskVM) =>
            {
                taskVM.InProgress = true;
                taskVM.Status = "Started Processing..";
                taskVM.OutputFormats.Where(oF => oF.Selected).ToList().ForEach((oF) => 
                {
                    MsgTaskUnit msgTaskUnit = new MsgTaskUnit(msg.Id, new MsgTaskUnitIds(msg.Id, taskVM.Id, oF.Id), fileInfoDict.GetValueOrDefault(taskVM.Id), oF.OutputFileType, msg.OutputFolder);
                    Worker.Tell(msgTaskUnit, ProgressUpdater);
                });
            });
        }

        private static bool ValidateInputTasks(MsgStartProcessing msg, Dictionary<Guid, FileInfo> fileInfoDict)
        {
            bool isAllTasksValid = true;
            foreach (var taskVM in msg.Tasks)
            {
                FileInfo fileInfo;
                if (String.IsNullOrEmpty(taskVM.SelectedFile))
                {
                    taskVM.Status = "Please Select a File";
                    taskVM.InProgress = isAllTasksValid = false;
                }
                else
                {
                    fileInfo = new FileInfo(taskVM.SelectedFile);
                    fileInfoDict.Add(taskVM.Id, fileInfo);
                    if (!fileInfo.Exists)
                    {
                        taskVM.Status = "The Selected File Doesn't Exist";
                        taskVM.InProgress = isAllTasksValid = false;
                    }
                    else if (taskVM.OutputFormats.Where(of => of.Selected).Count() == 0)
                    {
                        taskVM.Status = "Select Atleast One OutputFormat";
                        taskVM.InProgress = isAllTasksValid = false;
                    }
                }
            }

            return isAllTasksValid;
        }
    }
}
