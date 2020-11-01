using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Events
{
    public class TaskRemovedEvent
    {

        private TaskViewModel removedTask;

        public TaskViewModel RemovedTask
        {
            get { return removedTask; }
            set { removedTask = value; }
        }


        public TaskRemovedEvent(TaskViewModel removedTask)
        {
            this.removedTask = removedTask;
        }
    }
}
