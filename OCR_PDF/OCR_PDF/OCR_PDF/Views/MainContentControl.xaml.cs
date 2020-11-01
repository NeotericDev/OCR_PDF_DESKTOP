using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for MainContentControl.xaml
    /// </summary>
    public partial class MainContentControl : UserControl
    {
        public MainContentControl()
        {
            InitializeComponent();
        }

        private void TaskControl_RemoveTask(Events.TaskRemovedEvent taskRemovedEvent)
        {
            var vm = this.DataContext as MainContentControlViewModel;
            vm.RemoveTask(taskRemovedEvent.RemovedTask);
        }
    }
}
