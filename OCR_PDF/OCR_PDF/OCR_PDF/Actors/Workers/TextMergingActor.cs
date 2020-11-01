using Akka.Actor;
using Akka.Routing;
using OCR_PDF.Actors.UI;
using OCR_PDF.Core;
using OCR_PDF.Helpers;
using OCR_PDF.Messages;
using OCR_PDF.Models;
using OCRLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OCR_PDF.Actors.Workers
{
    public class TextMergingActor : CancellableStashingActor
    {
        private List<MsgMergeOCRdData> accumulatedList;
        private MsgDocToProcessOnTheWay msgDocToProcessOnTheWay;
        
        public TextMergingActor() : base()
        {
            accumulatedList = new List<MsgMergeOCRdData>();
            ReadyToWork();
        }

        private void ReadyToWork()
        {
            Receive<MsgDocToProcessOnTheWay>((msgDocToProcessTheWay) => {
                System.Diagnostics.Debug.WriteLine("Received MsgTaskUnitImage(WorkInProgresss) :: BatchId :: " + msgDocToProcessTheWay.Id);
                var sender = Sender;
            ProcessAsync(msgDocToProcessTheWay.TaskUnitId, ()=> 
                {
                    msgDocToProcessOnTheWay = msgDocToProcessTheWay;

                }, null);
                Become(WorkInProgress);
            });
            
        }
        private void WorkInProgress()
        {
            Receive<MsgMergeOCRdData>((msgMergeOCRdData) => {
                System.Diagnostics.Debug.WriteLine("Received MsgMergeOCRdData(WorkInProgresss) :: BatchId :: " + msgMergeOCRdData.Id);
                var sender = Sender;
                var self = Self;
                ProcessAsync(msgMergeOCRdData.TaskUnitId, () =>
                {
                    bool isAllCompleted = false;
                    accumulatedList.Add(msgMergeOCRdData);
                    if (msgDocToProcessOnTheWay.TotalPagesToExpect == accumulatedList.Count)
                    {
                        MsgTaskUnitProgress progressUpdate;
                        int pgCount = 0;
                        UTF8Encoding encoding = new UTF8Encoding(true);
                        var stage = Util.GetMergingrStage(msgDocToProcessOnTheWay.InputType);
                        string status;
                        using(var file = File.OpenWrite(msgDocToProcessOnTheWay.OutputFile))
                        {
                            accumulatedList.OrderBy((m) => m.Sequence).ToList().ForEach((oCRdMsg) => 
                            {
                                Byte[] dataBytes = encoding.GetBytes(oCRdMsg.Data);
                                WriteDataToFile(dataBytes, file);
                                Byte[] pageBoundry = encoding.GetBytes($"\n=================================Page {++pgCount} Ends=================================\n");
                                WriteDataToFile(pageBoundry, file);
                                if(pgCount == msgDocToProcessOnTheWay.TotalPagesToExpect)
                                {
                                    status = $"{pgCount}/{msgDocToProcessOnTheWay.TotalPagesToExpect} Pages Completed";
                                    isAllCompleted = true;
                                }
                                else
                                {
                                    status = $"Merged {pgCount}/{msgDocToProcessOnTheWay.TotalPagesToExpect} Pages..";
                                }
                                
                                progressUpdate = new MsgTaskUnitProgress(msgMergeOCRdData.Id, msgMergeOCRdData.TaskUnitId, true, stage, pgCount, msgDocToProcessOnTheWay.TotalPagesToExpect, OutputFileType.TEXT, status);
                                sender.Tell(progressUpdate);
                            });
                        }
                        if (isAllCompleted)
                        {
                            self.Tell(MsgDoneProcessing.GetInstance());
                            //BecomeAsReady();
                        }
                    }

                }, null);
            });

            Receive<MsgCancelAllTasks>((cancelAllTasksMsg) => {
                    System.Diagnostics.Debug.WriteLine("Received MsgCancelAllTasks(WorkInProgresss) :: BatchId :: " + cancelAllTasksMsg.Id);
                    CancelTokenForBatchId(cancelAllTasksMsg.Id);
                });
                Receive<MsgCancelTask>((cancelTask) => {
                    System.Diagnostics.Debug.WriteLine("Received MsgCancelTask(WorkInProgresss) :: TaskId :: " + cancelTask.Id);
                    CancelTokenForTask(cancelTask.TaskId);
                });
                Receive<MsgCancelTaskUnit>((cancelTask) => {
                    System.Diagnostics.Debug.WriteLine("Received MsgCancelTaskUnit(WorkInProgresss) :: TaskId :: " + cancelTask.TaskId + " TaskUnitId :: " + cancelTask.Id);
                    CancelToken(cancelTask.TaskId);
                }); 
            Receive<MsgDoneProcessing>((done) => {
                    System.Diagnostics.Debug.WriteLine("Received MsgDoneProcessing(WorkInProgresss) ");
                BecomeAsReady();
                });
            Receive<object>((cancelTask) => {
                    System.Diagnostics.Debug.WriteLine("Received Object(WorkInProgresss) :: Stashing it");
                    Stash.Stash();
                });
        }

        private void WriteDataToFile(Byte[] dataBytes, FileStream file)
        {
            file.Write(dataBytes, 0, dataBytes.Length);
        }

        private void BecomeAsReady()
        {
            Become(ReadyToWork);
            msgDocToProcessOnTheWay = null;
            accumulatedList.Clear();
        }
    }
}
