using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTask : MsgBase
    {
        public MsgTask(Guid id, Guid batchId, FileInfo inputFile, List<OutputModel> outputs)
        {
            Id = id;
            BatchId = batchId;
            InputFile = inputFile;
            Outputs = outputs;
        }

        public Guid BatchId { get; private set; }
        public FileInfo  InputFile { get; private set; }
        public List<OutputModel> Outputs { get; private set; }

    }
}
