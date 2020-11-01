using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Events
{
    public class PageCountAvailableEvent
    {
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public PageCountAvailableEvent()
        {
            this.EndPage = 0;
            this.StartPage = 0;
        }
        public PageCountAvailableEvent(int EndPage)
        {
            this.EndPage = EndPage;
            this.StartPage = 1;
        }
    }
}
