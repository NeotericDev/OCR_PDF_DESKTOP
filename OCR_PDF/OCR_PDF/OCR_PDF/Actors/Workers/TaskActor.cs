using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using OCR_PDF.Actors.UI;
using OCR_PDF.Core;
using OCR_PDF.Helpers;
using OCR_PDF.Messages;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCR_PDF.Actors.Workers
{
    public class TaskActor : CancellableStashingActor
    {
        private readonly Dictionary<OutputFileType, IActorRef> FileProcessActors;
        private readonly Dictionary<OutputFileType, IActorRef> FileMergingActors;
        private static readonly int BatchSize = 5;
        public TaskActor() : base()
        {
            //ReportTo = reportTo;
            var txtMergingActor = Context.ActorOf(Context.DI().Props<TextMergingActor>(), "TextMergingActor");
            var pdfMergingActor = Context.ActorOf(Context.DI().Props<TextMergingActor>(), "PDFMergingActor"); // TODO : Init proper Actor here
            var textActor = Context.ActorOf(TextActor.Props(txtMergingActor).WithRouter(FromConfig.Instance), "TextActor");
            var pdfActor = Context.ActorOf(TextActor.Props(pdfMergingActor).WithRouter(FromConfig.Instance), "PdfActor"); // TODO : Init proper Actor here
            FileProcessActors = new Dictionary<OutputFileType, IActorRef>();
            FileMergingActors = new Dictionary<OutputFileType, IActorRef>();
            FileProcessActors.Add(OutputFileType.TEXT, textActor);
            FileProcessActors.Add(OutputFileType.PDF, pdfActor);
            FileMergingActors.Add(OutputFileType.TEXT, txtMergingActor);
            FileMergingActors.Add(OutputFileType.PDF, pdfActor);
            ReadyToWork();
        }

        private void ReadyToWork()
        {
            Receive<MsgTaskUnit>((taskMsg) => {                
                System.Diagnostics.Debug.WriteLine("Received MsgTask : " + taskMsg.Id);
                var self = Self;
                var sender = Sender;
                ProcessAsync(taskMsg.TaskUnitId, () =>
                {
                    InputFileType iFileType = Util.GetInputFileType(taskMsg.InputFile);
                    List<FileInfo> files;
                    MsgDocToProcessOnTheWay msgDocToProcessOnTheWay;
                    string outputFile = taskMsg.OutputFolder + @"\" + taskMsg.InputFile.Name + ".txt";
                    string tmpImgFolder = Path.Combine(taskMsg.OutputFolder, "IMAGES");
                    Directory.CreateDirectory(tmpImgFolder);
                    if (iFileType == InputFileType.IMAGE)
                    {
                        files = new List<FileInfo>();
                        files.Add(taskMsg.InputFile);
                        msgDocToProcessOnTheWay = new MsgDocToProcessOnTheWay(taskMsg.Id, taskMsg.TaskUnitId, files.Count, outputFile, iFileType);
                        FileMergingActors.GetValueOrDefault(taskMsg.OutputFileType).Tell(msgDocToProcessOnTheWay, sender);
                        MsgTaskUnitImage msgTaskUnitImage = new MsgTaskUnitImage(1, taskMsg.Id, taskMsg.TaskUnitId, files, iFileType, BatchSize, taskMsg.InputFile.Name, 1);
                        FileProcessActors.GetValueOrDefault(taskMsg.OutputFileType).Tell(msgTaskUnitImage, sender);
                    }
                    else
                    {
                        files = new List<FileInfo>();
                        PdfLoadedDocument doc = new PdfLoadedDocument(taskMsg.InputFile.OpenRead());
                        int TotalPagesToProcess = doc.Pages.Count;
                        msgDocToProcessOnTheWay = new MsgDocToProcessOnTheWay(taskMsg.Id, taskMsg.TaskUnitId, TotalPagesToProcess, outputFile, iFileType);
                        FileMergingActors.GetValueOrDefault(taskMsg.OutputFileType).Tell(msgDocToProcessOnTheWay, sender);
                        int i = 0, batchNum = 0;
                        foreach(var file in ConvertPagesToImageFiles(doc, tmpImgFolder, taskMsg.InputFile.Name))
                        {
                            ++i;
                            MsgTaskUnitProgress progressUpdate = new MsgTaskUnitProgress(taskMsg.Id, taskMsg.TaskUnitId, true, ProcessStage.PDF_IMG_EXTRACTION, i, TotalPagesToProcess, taskMsg.OutputFileType, $"Converted {i}/{TotalPagesToProcess} Pages into Image");
                            sender.Tell(progressUpdate);
                            if (i < BatchSize)
                            {
                                files.Add(file);
                            }
                            else
                            {
                                files.Add(file);
                                SendPartialFileList(taskMsg, iFileType, files, ++batchNum, BatchSize, TotalPagesToProcess, sender);
                                files = new List<FileInfo>();
                                i = 0;
                            }                            
                        }
                        if(files.Count > 0)
                        {
                            SendPartialFileList(taskMsg, iFileType, files, ++batchNum, BatchSize, TotalPagesToProcess, sender);
                        }
                    }                   

                }, null);
                Become(WorkInProgresss);
            });
        }

        private void SendPartialFileList(MsgTaskUnit taskMsg, InputFileType iFileType, List<FileInfo> files, int batchNum, int BatchSize, int totalPages, IActorRef sender)
        {
            MsgTaskUnitImage msgTaskUnitImage = new MsgTaskUnitImage(batchNum, taskMsg.Id, taskMsg.TaskUnitId, files, iFileType, BatchSize, taskMsg.InputFile.Name, totalPages);
            FileProcessActors.GetValueOrDefault(taskMsg.OutputFileType).Tell(msgTaskUnitImage, sender);
        }

        private int GetPageCount(FileInfo inputFile) => new PdfLoadedDocument(inputFile.OpenRead()).Pages.Count;

        private IEnumerable<FileInfo> ConvertPagesToImageFiles(PdfLoadedDocument doc, string path, string fileName)
        {
            int pageCount = doc.Pages.Count;
            string fName;
            for (int i=0; i < pageCount; i++)
            {
                Bitmap bitmap = doc.ExportAsImage(i);
                fName = path + @"\" + fileName + i + ".jpg";
                //bitmap.Save(fName, ImageFormat.Jpeg);
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = File.OpenWrite(fName))
                    {
                        bitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                yield return new FileInfo(fName);
            }
        }

        private void WorkInProgresss()
        {

            Receive<MsgCancelAllTasks>((cancelAllTasksMsg) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelAllTasks(WorkInProgresss) :: BatchId :: " + cancelAllTasksMsg.Id);
                CancelTokenForBatchId(cancelAllTasksMsg.Id);
                SendToAllSubordinates(FileProcessActors.Values, new Broadcast(cancelAllTasksMsg));
                SendToAllSubordinates(FileMergingActors.Values, cancelAllTasksMsg);
            });
            Receive<MsgCancelTask>((cancelTask) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTask(WorkInProgresss) :: TaskId :: " + cancelTask.Id);
                CancelTokenForTask(cancelTask.TaskId);
                SendToAllSubordinates(FileProcessActors.Values, new Broadcast(cancelTask));
                SendToAllSubordinates(FileMergingActors.Values, cancelTask);
            });
            Receive<MsgCancelTaskUnit>((cancelTask) => {
                System.Diagnostics.Debug.WriteLine("Received MsgCancelTaskUnit(WorkInProgresss) :: TaskId :: " + cancelTask.TaskId + " TaskUnitId :: " + cancelTask.Id);
                CancelToken(cancelTask.TaskId);
            });
            Receive<object>((anyMsg) => {
                System.Diagnostics.Debug.WriteLine("Received Object(WorkInProgresss) :: Stashing it");
                Stash.Stash();
            });

        }

        private void SendToAllSubordinates(Dictionary<OutputFileType, IActorRef>.ValueCollection actorRefs, object msg)
        {
            foreach (var actor in actorRefs)
            {
                actor.Tell(msg, Sender);
            }
        }

        private void BecomeAsReady()
        {
            Become(ReadyToWork);
        }
    }
}
