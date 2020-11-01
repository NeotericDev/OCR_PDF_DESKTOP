using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using OCR_PDF.Actors.Workers;
using OCR_PDF.Helpers;
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
    public class ProgressUpdater : CancellableStashingActor
    {
        Dictionary<Guid, MsgBeReadyToGetUpdates> dataDict;
        Dictionary<string, int> pageCountDict;
        public ProgressUpdater() : base()
        {
            System.Diagnostics.Debug.WriteLine(Self.Path + " |=| UiCoordinatorActor :: Dispatcher In Use : " + Context.Dispatcher.Id + " | " + Context.Dispatcher.GetType().Name + " | " + Context.Dispatcher.ToString());
            // actor = Context.ActorOf(Context.DI().Props<ChildActor>(), "ChildActor");
            dataDict = new Dictionary<Guid, MsgBeReadyToGetUpdates>();
            pageCountDict = new Dictionary<string, int>();
            ReadyToStartWork();
        }

        public void ReadyToStartWork() 
        {
            Receive<MsgBeReadyToGetUpdates>((msg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgBeReadyToGetUpdates(ReadyToStartWork) : " + msg.Id);
                dataDict.Add(msg.Id, msg);
            });
            Receive<MsgCancelAllTasks>((cancelAllTasksMsg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelAllTasks(WorkInProgresss) :: BatchId :: " + cancelAllTasksMsg.Id);
                //CancelTokenForBatchId(cancelAllTasksMsg.Id);
                ProcessAsync(cancelAllTasksMsg.Id, () =>
                {
                    // TODO : Update all TaskVMs for cancelled
                    var dataModelVM = dataDict.GetValueOrDefault(cancelAllTasksMsg.Id);
                    foreach (var taskVM in dataModelVM.Tasks)
                    {
                        MaarkAsInterrupted(taskVM);
                    }
                    dataDict.Remove(cancelAllTasksMsg.Id);
                }, null);
            });
            Receive<MsgCancelTask>((cancelTask) =>
            {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTask(WorkInProgresss) :: TaskId :: " + cancelTask.Id);
                var dataModelVM = dataDict.GetValueOrDefault(cancelTask.Id);
                foreach (var taskVM in dataModelVM.Tasks)
                {
                    if (taskVM.Id.Equals(cancelTask.TaskId))
                    {
                        MaarkAsInterrupted(taskVM);
                        break;
                    }
                }
            });
            Receive<MsgCancelTaskUnit>((cancelTask) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTaskUnit(WorkInProgresss) :: TaskId :: " + cancelTask.TaskId + " TaskUnitId :: " + cancelTask.Id);
                var dataModelVM = dataDict.GetValueOrDefault(cancelTask.Id);
                var breakOuter = false;
                foreach (var taskVM in dataModelVM.Tasks)
                {
                    if (taskVM.Id.Equals(cancelTask.TaskId))
                    {
                        foreach (var taskUnitVM in taskVM.OutputFormats)
                        {
                            if (taskUnitVM.Id.Equals(cancelTask.TaskUnitId))
                            {
                                MarkAsInterrupted(taskUnitVM);
                                breakOuter = true;
                                break;
                            }
                        }

                        if (breakOuter)
                        {
                            break;
                        }
                    }
                }
            });
            Receive<MsgTaskUnitProgress>((taskProgressMsg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgTaskProgress(WorkInProgresss) : " + taskProgressMsg.Id);
                //pageCount
                int completedPages = 0;
                if (pageCountDict.ContainsKey(taskProgressMsg.TaskUnitId.ToString()))
                {
                    completedPages = pageCountDict.GetValueOrDefault(taskProgressMsg.TaskUnitId.ToString());
                    pageCountDict.Remove(taskProgressMsg.TaskUnitId.ToString());
                }
                ++completedPages;
                pageCountDict.Add(taskProgressMsg.TaskUnitId.ToString(), completedPages);

                var prgrs = (int)Util.CalculateProgress(taskProgressMsg.Stage, completedPages, taskProgressMsg.TotalPages);
                var dataModelVM = dataDict.GetValueOrDefault(taskProgressMsg.Id);
                if (!taskProgressMsg.IsInterrupted)
                {
                    var taskVM = dataModelVM.Tasks?.Where(tVM => taskProgressMsg.TaskUnitId.TaskId.Equals(tVM.Id)).First();
                    var taskOpVMs = taskVM.OutputFormats;
                    var taskOpVM = taskOpVMs.Where(oF => oF.Id.Equals(taskProgressMsg.TaskUnitId.Id)).First();
                    string status = "";
                    switch (taskProgressMsg.Stage)
                    {
                        case Core.ProcessStage.PDF_IMG_EXTRACTION:
                            status = $"Converted {completedPages}/{taskProgressMsg.TotalPages} Pages into Image";
                            break;
                        case Core.ProcessStage.PDF_IMG_OCR:
                        case Core.ProcessStage.IMG_OCR:
                            status = $"Extracted data from {completedPages}/{taskProgressMsg.TotalPages} Pages..";
                            break;
                        case Core.ProcessStage.PDF_TXT_PDF_MERGING:
                        case Core.ProcessStage.IMG_TXT_PDF_MERGING:
                            status = $"Completed {completedPages}/{taskProgressMsg.TotalPages} Pages..";
                            break;
                    }
                    taskOpVM.CurrentCompletionStatus = status;
                    taskOpVM.Progress = prgrs;
                    //taskOpVM.CurrentCompletionStatus = taskProgressMsg.Status;
                    if (prgrs == 100 && taskOpVMs.ToList().All(ofVM => ofVM.Progress == 100))
                    {
                        taskVM.InProgress = false;
                        taskVM.Status = "Completed";
                    }
                }
            });
        }

        private static void MaarkAsInterrupted(ViewModels.TaskViewModel tVM)
        {
            tVM.InProgress = false;
            tVM.Status = "Cancelled";
            tVM.OutputFormats.ToList().ForEach(oF => MarkAsInterrupted(oF));
        }

        private static void MarkAsInterrupted(ViewModels.OutputFormatViewModel oF)
        {
            oF.Interrupted = true;
            oF.CurrentCompletionStatus = "Cancelled..";
        }

        private void BecomeAsReady()
        {
            Become(ReadyToStartWork);
        }
    }
}
