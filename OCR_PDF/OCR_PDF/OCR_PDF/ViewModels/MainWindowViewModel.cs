using Prism.Commands;
using Prism.Mvvm;

namespace OCR_PDF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Asker's OCR Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            MainVM = new MainContentControlViewModel();
            HeaderVM = new HeaderControlViewModel();
            HeaderVM.NewFileClicked += () => MainVM.AddNewTask();
            HeaderVM.StartProcessingClicked += () => MainVM.StartProcessing();
            MainVM.TaskCountUpdated += (count) => HeaderVM.TaskCount = count;

        }

        private HeaderControlViewModel headerVm;
        public HeaderControlViewModel HeaderVM
        {
            get { return headerVm; }
            set { SetProperty(ref headerVm, value); }
        }

        private MainContentControlViewModel mainVm;
        public  MainContentControlViewModel MainVM
        {
            get { return mainVm; }
            set { SetProperty(ref mainVm, value); }
        }

    }
}
