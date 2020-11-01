using OCR_PDF.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Core
{
    public enum OutputFileType
    {
        [FileType("PDF", "pdf")] PDF,
        [FileType("Text", "txt")] TEXT
    }
}
