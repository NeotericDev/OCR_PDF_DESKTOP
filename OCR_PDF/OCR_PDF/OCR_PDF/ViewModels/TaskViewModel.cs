using Microsoft.Win32;
using OCR_PDF.Events;
using OCR_PDF.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OCR_PDF.ViewModels
{
    public class TaskViewModel: BindableBase
    {
        public TaskViewModel()
        {
            this.Id = Guid.NewGuid();
            InputLanguagesSupported = new ObservableCollection<LanguageViewModel>(Util.GetAvailableInputLanguagesVMs(@"C:\Users\Dev\Documents\tessdata"));
            OutputFormats = new ObservableCollection<OutputFormatViewModel>(Util.GetAvailableOutputFormatsVMs());
        }

        public Guid Id { get; private set; }

        private string selectedFile;
        public string SelectedFile
        {
            get { return selectedFile; }
            set { SetProperty(ref selectedFile, value); }
        }

        private bool inProgress;
        public bool InProgress
        {
            get { return inProgress; }
            set { SetProperty(ref inProgress, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private int fromPage;
        public int FromPage
        {
            get { return fromPage; }
            set { SetProperty(ref fromPage, value); }
        }

        private int toPage;
        public int ToPage
        {
            get { return toPage; }
            set { SetProperty(ref toPage, value); }
        }

        private int endPage;
        public int EndPage
        {
            get { return endPage; }
            set { SetProperty(ref endPage, value); UpdateMultipageVariable(); }
        }

        private bool isMultiPageDocument;
        public bool IsMultiPageDocument
        {
            get { return isMultiPageDocument; }
            set { SetProperty(ref isMultiPageDocument, value); }
        }

        private ObservableCollection<OutputFormatViewModel> outputFormats;
        public ObservableCollection<OutputFormatViewModel> OutputFormats
        {
            get { return outputFormats; }
            private set { SetProperty(ref outputFormats, value); }
        }

        private ObservableCollection<LanguageViewModel> inputLanguagesSupported;
        public ObservableCollection<LanguageViewModel> InputLanguagesSupported
        {
            get { return inputLanguagesSupported; }
            private set { SetProperty(ref inputLanguagesSupported, value); }
        }

        private bool isImage;
        public bool IsImage
        {
            get { return isImage; }
            set { SetProperty(ref isImage, value); }
        }

        private bool isPDF;
        public bool IsPDF
        {
            get { return isPDF; }
            set { SetProperty(ref isPDF, value); }
        }

        private Prism.Commands.DelegateCommand chooseFileCmd;
        public Prism.Commands.DelegateCommand ChooseFileCmd =>
            chooseFileCmd ?? (chooseFileCmd = new Prism.Commands.DelegateCommand(ExecuteChooseFileCmd, CanExecuteChooseFileCmd));

        void ExecuteChooseFileCmd()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Supported Files (.pdf, .jpeg, .jpg, .png)|*.pdf;*.jpeg;*.jpg;*.png|PDF File (.pdf)|*.pdf|Image Files (*.jpeg, *.jpg, *.png)|*.jpeg;*.jpg;*.png";
            if(openFileDialog.ShowDialog() == true)
            {
                SelectedFile = openFileDialog.FileName;
                if(SelectedFile.IndexOf(".pdf") > 0)
                {
                    IsImage = false;
                    IsPDF = true;
                    //this.ToPage = pdfViewer.PageCount;
                    //this.IsMultiPageDocument = this.FromPage < this.ToPage;
                    //PdfDocumentView pdfViewer = new PdfDocumentView();
                    //pdfViewer.Load(f);
                    ////pdfViewer.RenderSize = new Size(200, 300);
                    ////pdfViewer.MaximumZoomPercentage = 300;
                    ////pdfViewer.Height = 100;
                    ////pdfViewer.Width = 200;
                    ////pdfViewer.ZoomTo(20);
                    ////pdfViewer.MaxHeight = 300;
                    ////pdfViewer.ZoomTo(300);
                    //pdfViewer.ZoomMode = ZoomMode.FitWidth;
                    //pdfViewer.ShowHorizontalScrollBar = false;
                    //pdfViewer.ShowVerticalScrollBar = false;
                    //pdfViewer.ShowScrollbar = false;
                    //pdfViewer.IsEnabled = false;

                    //this.FilePreviewControl = pdfViewer;
                }
                else
                {
                    IsImage = true;
                    IsPDF = false;
                    //Image image = new Image();
                    //image.Stretch = Stretch.UniformToFill;
                    //image.Source = new BitmapImage(new Uri(SelectedFile));
                    //this.FilePreviewControl = image;
                    //this.IsMultiPageDocument = false;
                }
                
            }
        }

        private void UpdateMultipageVariable()
        {
            IsMultiPageDocument = FromPage < EndPage;
        }

        bool CanExecuteChooseFileCmd()
        {
            return true;
        }

        public void UpdatePageCount(int EndPage)
        {
            this.EndPage = EndPage;
            this.ToPage = EndPage;
            this.IsMultiPageDocument = this.FromPage < this.EndPage;
        }

    }
}
