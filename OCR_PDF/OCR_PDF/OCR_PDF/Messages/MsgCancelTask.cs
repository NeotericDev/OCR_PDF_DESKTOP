using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgCancelTask : MsgBase
    {
        public MsgCancelTask(Guid batchId, Guid taskId)
        {
            Id = batchId;
            TaskId = taskId;
        }
        public Guid TaskId { get; private set; }
    }
}
