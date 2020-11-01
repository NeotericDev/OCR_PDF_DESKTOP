using OCR_PDF.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Core
{
    public enum ProcessStage
    {
        [Percentage(33, 0)] PDF_IMG_EXTRACTION,
        [Percentage(33, 33)] PDF_IMG_OCR,
        [Percentage(34, 66)] PDF_TXT_PDF_MERGING,
        [Percentage(50, 0)] IMG_OCR,
        [Percentage(50, 50)] IMG_TXT_PDF_MERGING
    }
}
