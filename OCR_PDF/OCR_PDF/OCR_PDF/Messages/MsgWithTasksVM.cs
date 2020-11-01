using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgWithTasksVM : MsgBase
    {
        public ObservableCollection<TaskViewModel> Tasks { get; private set; }

        public MsgWithTasksVM(Guid id, ObservableCollection<TaskViewModel> tasks)
        {
            Id = id;
            Tasks = tasks;
        }
    }
}
