using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTaskUnit : MsgBase
    {
        public MsgTaskUnit(Guid id, MsgTaskUnitIds taskUnitId, FileInfo inputFile, OutputFileType outputFileType, string outputFolder)
        {
            Id = id;
            TaskUnitId = taskUnitId;
            InputFile = inputFile;
            OutputFileType = outputFileType;
            OutputFolder = outputFolder;
        }

        public MsgTaskUnitIds TaskUnitId { get; private set; }
        public FileInfo  InputFile { get; private set; }
        public OutputFileType OutputFileType{ get; private set; }
        public string OutputFolder { get; private set; }

    }
}
