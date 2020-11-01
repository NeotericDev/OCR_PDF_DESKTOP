using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgCancelTaskUnit : MsgBase
    {
        public MsgCancelTaskUnit(Guid id, Guid taskId, Guid taskUnitId)
        {
            Id = id;
            TaskId = taskId;
            TaskUnitId = taskUnitId;
        }
        public Guid TaskId { get; private set; }
        public Guid TaskUnitId { get; private set; }
    }
}
