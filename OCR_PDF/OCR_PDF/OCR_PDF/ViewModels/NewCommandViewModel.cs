using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.ViewModels
{
    public class NewCommandViewModel : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private DelegateCommand actionCommand;
        public DelegateCommand ActionCommand
        {
            get { return actionCommand; }
            set { SetProperty(ref actionCommand, value); }
        }
    }
}
