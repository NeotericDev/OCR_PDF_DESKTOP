using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgCancelAllTasks : MsgBase
    {
        public MsgCancelAllTasks(Guid id)
        {
            Id = id;
        }
    }
}
