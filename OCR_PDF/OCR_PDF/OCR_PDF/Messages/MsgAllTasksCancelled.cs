using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgAllTasksCancelled : MsgBase
    {
        public MsgAllTasksCancelled(Guid id)
        {
            Id = id;
        }
    }
}
