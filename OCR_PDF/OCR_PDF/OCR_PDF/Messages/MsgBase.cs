using Akka.Routing;
using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public abstract class MsgBase
    {
        public Guid Id { get; protected set; }
    }
}
