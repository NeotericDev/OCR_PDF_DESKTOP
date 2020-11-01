using OCR_PDF.Events;
using OCR_PDF.ViewModels;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCR_PDF.Views
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        public TaskControl()
        {
            InitializeComponent();
        }

        public delegate void RemoveTaskHandler(TaskRemovedEvent taskRemovedEvent);
        public event RemoveTaskHandler RemoveTask;

        public delegate void DocumentPageCountHandler(PageCountAvailableEvent pageCountAvailableEvent);
        public event DocumentPageCountHandler DocumentPageCountAvailable;

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            var tvm = (TaskViewModel)this.DataContext;
            TaskRemovedEvent taaskRemovedEvent = new TaskRemovedEvent(tvm);
            if(RemoveTask != null)
            {
                RemoveTask(taaskRemovedEvent);
            }
        }

        private void pdfViewer_DocumentLoaded(object sender, EventArgs args)
        {
            var pdfViewer = sender as PdfDocumentView;
            int endPage = pdfViewer != null ? pdfViewer.PageCount : 0;
            var vm = this.DataContext as TaskViewModel;
            vm.UpdatePageCount(endPage);
        }
    }
}
