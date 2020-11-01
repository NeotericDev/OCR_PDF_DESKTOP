using Akka.Routing;
using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgDocToProcessOnTheWay : MsgBase
    {
        public MsgDocToProcessOnTheWay(Guid id, MsgTaskUnitIds taskUnitId, int totalPagesToExpect, string outputFile, InputFileType inputType)
        {
            Id = id;
            TaskUnitId = taskUnitId;
            TotalPagesToExpect = totalPagesToExpect;
            OutputFile = outputFile;
            InputType = inputType;
        }

        public MsgTaskUnitIds TaskUnitId { get; private set; }
        public int TotalPagesToExpect { get; private set; }
        public string OutputFile { get; private set; }

        public InputFileType InputType { get; private set; }
    }
}
