using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTaskCancelled : MsgBase
    {
        public MsgTaskCancelled(Guid id, Guid taskId)
        {
            Id = id;
            TaskId = taskId;
        }

        public Guid TaskId { get; private set; }
    }
}
