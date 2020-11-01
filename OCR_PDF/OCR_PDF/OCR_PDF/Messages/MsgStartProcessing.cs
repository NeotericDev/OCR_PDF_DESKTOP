using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace OCR_PDF.Messages
{
    public class MsgStartProcessing : MsgWithTasksVM
    {
        public Object Resource { get; private set; }
        public string OutputFolder { get; private set; }

        public MsgStartProcessing(Guid id, ObservableCollection<TaskViewModel> tasks, Object resource): this(id, tasks, resource, GetNewTempPath())
        {
        }
        public MsgStartProcessing(Guid id, ObservableCollection<TaskViewModel> tasks, Object resource, string outputFolder) : base(id, tasks)
        {
            Resource = resource;
            OutputFolder = string.IsNullOrEmpty(outputFolder) ? GetNewTempPath() : outputFolder;
        }

        private static string GetNewTempPath()
        {
            string newTempPath = Path.Combine(Path.GetTempPath(), GetDateTimeForForFileFolder());
            Directory.CreateDirectory(newTempPath);
            return newTempPath;
        }

        private static string GetDateTimeForForFileFolder()
        {
            DateTime dateTime = DateTime.Now;
            StringBuilder builder = new StringBuilder();
            return builder
                .Append(dateTime.Day)
                .Append("_")
                .Append(dateTime.Month)
                .Append("_")
                .Append(dateTime.Year)
                .Append("__")
                .Append(dateTime.Hour)
                .Append("_")
                .Append(dateTime.Minute)
                .Append("_")
                .Append(dateTime.Second)
                .Append("_")
                .Append(dateTime.Millisecond)
                .ToString();
        }
    }
}
