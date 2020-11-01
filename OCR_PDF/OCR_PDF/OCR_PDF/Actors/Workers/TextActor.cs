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
    public class TextActor : CancellableStashingActor
    {
        private IActorRef MergingActor;
        private ImageOcr ocrEngine;
        public TextActor(IActorRef mergerActor) : base()
        {
            MergingActor = mergerActor;
            ocrEngine = new ImageOcr(Util.GetTessBinariesPath(), SharedData.TessDataPath);
            ReadyToWork();
        }

        public static Props Props(IActorRef mergingActor)
        {            
            return Akka.Actor.Props.Create<TextActor>(mergingActor);
        }

        private void ReadyToWork()
        {
            Receive<MsgTaskUnitImage>((msgTaskImg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgTaskUnitImage(WorkInProgresss) :: BatchId :: " + msgTaskImg.Id);
                var sender = Sender;
            ProcessAsync(msgTaskImg.TaskUnitId, ()=> 
                {
                    int fNum = 0;
                    string ocredData = string.Empty;
                    int processedCount = (msgTaskImg.BatchSize * (msgTaskImg.SeqBatchNumber - 1));
                    msgTaskImg.InputFiles.ForEach((inputFile) =>
                    {
                        ++processedCount;
                        Bitmap bitmap = new Bitmap(inputFile.FullName);
                        ocredData = ocrEngine.DoOCR(bitmap);
                        MsgMergeOCRdData msgMergeOCRd = new MsgMergeOCRdData(msgTaskImg.Id, msgTaskImg.TaskUnitId, processedCount, ocredData);
                        MergingActor.Tell(msgMergeOCRd, sender);
                        MsgTaskUnitProgress progressUpdate = new MsgTaskUnitProgress(msgTaskImg.Id, msgTaskImg.TaskUnitId, true, Util.GetImgOcrStage(msgTaskImg.InputType), processedCount, msgTaskImg.TotalPages, OutputFileType.TEXT, $"Extracted data from {processedCount}/{msgTaskImg.TotalPages} Pages");
                        sender.Tell(progressUpdate);
                    });
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
            Receive<object>((cancelTask) => {
                System.Diagnostics.Debug.WriteLine("Received Object(WorkInProgresss) :: Stashing it");
                Stash.Stash();
            });

        }

        private void BecomeAsReady()
        {
            Become(ReadyToWork);
        }
    }
}
