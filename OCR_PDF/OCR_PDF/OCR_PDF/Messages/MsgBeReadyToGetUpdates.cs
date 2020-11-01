using Akka.Routing;
using OCR_PDF.Core;
using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgBeReadyToGetUpdates : MsgWithTasksVM
    {
        public MsgBeReadyToGetUpdates(Guid id, ObservableCollection<TaskViewModel> tasks) : base(id, tasks)
        {
        }
    }
}
