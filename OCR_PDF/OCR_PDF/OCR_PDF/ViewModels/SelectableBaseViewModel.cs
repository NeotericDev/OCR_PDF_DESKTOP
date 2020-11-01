using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.ViewModels
{
    public class SelectableBaseViewModel : BindableBase
    {

        public SelectableBaseViewModel(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        private Guid id;
        public Guid Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            private set { SetProperty(ref name, value); }
        }
    }
}
