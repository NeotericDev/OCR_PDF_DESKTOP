using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Messages
{
    public class OutputModel : MsgBase
    {
        public OutputModel(Guid id, OutputFileType outputFileType)
        {
            Id = id;
            OutputFileType = outputFileType;
        }

        public OutputFileType OutputFileType { get; private set; }
    }
}
