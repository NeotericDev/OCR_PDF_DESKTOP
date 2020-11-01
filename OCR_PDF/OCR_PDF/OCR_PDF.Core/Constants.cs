using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Core
{
    public static class Constants
    {
        public static readonly IEnumerable<OutputFileType> OUTPUT_FILE_TYPES = OutputFileType.TEXT.GetValues();
        public static readonly string TESS_DATA_FILE_EXTENSION = "traineddata";
    }
}
