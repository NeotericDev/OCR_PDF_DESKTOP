using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Core.Attributes
{
    public class FileTypeAttribute : Attribute
    {
        public FileTypeAttribute(string name, string ext) => (Name, Extension) = (name, ext);
        public string Extension { get; private set; }
        public string Name { get; private set; }
    }
}
