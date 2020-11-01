using OCR_PDF.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.ViewModels
{
    public class OutputFormatViewModel : SelectableBaseViewModel
    {
        private int progress;
        public int Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }

        private bool interrupted;
        public bool Interrupted
        {
            get { return interrupted; }
            set { SetProperty(ref interrupted, value); }
        }

        private int progressSegment;
        public int ProgressSegment
        {
            get { return progressSegment; }
            set { SetProperty(ref progressSegment, value); }
        }

        private OutputFileType outputFileType;
        public OutputFileType OutputFileType
        {
            get { return outputFileType; }
            set { SetProperty(ref outputFileType, value); }
        }

        private string currentCompletionStatus;

        public OutputFormatViewModel(OutputFileType outputFileType) : base(outputFileType.GetName())
        {
            OutputFileType = outputFileType;
            switch (outputFileType)
            {
                case OutputFileType.PDF:
                    ProgressSegment = 3;
                    break;
                case OutputFileType.TEXT:
                    ProgressSegment = 1;
                    break;
            }
        }

        public string CurrentCompletionStatus
        {
            get { return currentCompletionStatus; }
            set { SetProperty(ref currentCompletionStatus, value); }
        }

    }
}
