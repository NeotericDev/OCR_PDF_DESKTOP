using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgDoneProcessing
    {
        private static MsgDoneProcessing Instance;
        private MsgDoneProcessing() { 
        }
        public static MsgDoneProcessing GetInstance() => Instance ?? (Instance = new MsgDoneProcessing());
    }
}
