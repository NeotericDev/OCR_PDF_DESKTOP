using OCR_PDF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgTaskUnitIds : MsgBase
    {
        public MsgTaskUnitIds(Guid batchId, Guid taskId, Guid taskUnitId )
        {
            Id = taskUnitId;
            BatchId = batchId;
            TaskId = taskId;
        }

        public Guid BatchId { get; private set; }
        public Guid TaskId { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is MsgTaskUnitIds ids &&
                   Id.Equals(ids.Id) &&
                   BatchId.Equals(ids.BatchId) &&
                   TaskId.Equals(ids.TaskId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BatchId, TaskId);
        }

        public bool BatchEquals(MsgTaskUnitIds obj)
        {
            return obj != null && BatchId.Equals(obj.BatchId);
        }

        public bool BatchEquals(Guid id)
        {
            return id != null && BatchId.Equals(id);
        }

        public bool TaskEquals(Guid id, Guid taskId)
        {
            return id != null && BatchId.Equals(id) && TaskId.Equals(taskId);
        }

        public bool TaskEquals(MsgTaskUnitIds obj)
        {
            return obj != null && BatchId.Equals(obj.BatchId) && TaskId.Equals(obj.TaskId);
        }

        public override string ToString()
        {
            return Id.ToString() + "_" + TaskId.ToString() + "_" + BatchId.ToString();
        }
    }
}
