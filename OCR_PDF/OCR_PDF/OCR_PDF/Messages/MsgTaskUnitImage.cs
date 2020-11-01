using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTaskUnitImage : MsgBase
    {
        public MsgTaskUnitImage(int seqBatchNum, Guid id, MsgTaskUnitIds taskUnitId, List<FileInfo> inputFiles,
            InputFileType inputFileType, int batchSize, string fileNameToUse, int totalPages) 
        {
            SeqBatchNumber = seqBatchNum;
            Id = id;
            TaskUnitId = taskUnitId;
            InputFiles = inputFiles;
            InputType = inputFileType;
            BatchSize = batchSize;
            FileNameToUse = fileNameToUse;
            TotalPages = totalPages;
        }

        public int BatchSize { get; set; }
        public int SeqBatchNumber { get; set; }
        public MsgTaskUnitIds TaskUnitId { get; private set; }
        public InputFileType InputType { get; private set; }
        public List<FileInfo> InputFiles { get; private set; }
        public string FileNameToUse { get; private set; }
        public int TotalPages { get; private set; }
    }
}
