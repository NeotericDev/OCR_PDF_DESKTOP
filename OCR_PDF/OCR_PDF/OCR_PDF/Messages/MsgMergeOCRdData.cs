using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgMergeOCRdData : MsgBase
    {
        public MsgMergeOCRdData(Guid id, MsgTaskUnitIds taskUnitIds, int sequence, string data)
        {
            Id = id;
            Sequence = sequence;
            Data = data;
            TaskUnitId = taskUnitIds;
        }

        public int Sequence { get; set; }
        public string Data { get; set; }

        public MsgTaskUnitIds TaskUnitId { get; private set; }
    }
}
