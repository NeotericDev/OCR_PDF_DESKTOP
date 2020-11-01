using OCR_PDF.Core;
using OCR_PDF.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTaskUnitProgress : MsgBase
    {
        public MsgTaskUnitProgress(Guid batchId, MsgTaskUnitIds taskUnitId, bool completed, ProcessStage stage, int completedPages,
            int totalPages, OutputFileType outputFileType, string status)
        {
            Id = batchId;
            TaskUnitId = taskUnitId;
            Completed = completed;
            Progress = (int)Util.CalculateProgress(stage, completedPages, totalPages);
            CompletedPages = completedPages;
            TotalPages = totalPages;
            OutputType = outputFileType;
            Status = status;
        }

        public MsgTaskUnitProgress(Guid batchId, MsgTaskUnitIds taskUnitId, bool completed, ProcessStage stage, int completedPages,
            int totalPages, bool isInterrupted, OutputFileType outputFileType, string status) 
            : this(batchId, taskUnitId, completed, stage, completedPages, totalPages, outputFileType, status)
        {
            IsInterrupted = isInterrupted;
        }
        public MsgTaskUnitIds TaskUnitId { get; private set; }
        public bool Completed { get; private set; }

        public int Progress { get; private set; }
        public int CompletedPages { get; private set; }
        public int TotalPages { get; private set; }
        public bool IsInterrupted { get; private set; }
        public string Status { get; private set; }
        public OutputFileType OutputType { get; private set; }
        public ProcessStage Stage { get; private set; }
    }
}
