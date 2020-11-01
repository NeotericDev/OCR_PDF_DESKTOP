using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.ViewModels
{
    public class HeaderControlViewModel : BindableBase
    {
        public HeaderControlViewModel()
        {
        }

        private Int32 taskCount;
        public Int32 TaskCount
        {
            get { return taskCount; }
            set { SetProperty(ref taskCount, value); }
        }


        private DelegateCommand startProcessingCmd;
        public DelegateCommand StartProcessingCmd =>
            startProcessingCmd ?? (startProcessingCmd = new DelegateCommand(ExecuteStartProcessingCmd, CanExecuteStartProcessingCmd).ObservesProperty(() => TaskCount));

        void ExecuteStartProcessingCmd()
        {
            StartProcessingClicked?.Invoke();
        }

        bool CanExecuteStartProcessingCmd()
        {
            return TaskCount > 0;
        }

        private DelegateCommand addFileBtnCmd;
        public DelegateCommand AddFileBtnCmd =>
            addFileBtnCmd ?? (addFileBtnCmd = new DelegateCommand(ExecuteAddFileBtnCmd, CanExecuteAddFileBtnCmd));

        void ExecuteAddFileBtnCmd()
        {
            NewFileClicked?.Invoke();
        }

        bool CanExecuteAddFileBtnCmd()
        {
            return true;
        }

        public delegate void NewFileClickedHandler();
        public event NewFileClickedHandler NewFileClicked; 
        public delegate void StartProcessingClickedHandler();
        public event StartProcessingClickedHandler StartProcessingClicked;
    }
}
