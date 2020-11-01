using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_PDF.Core.Attributes
{
    public class PercentageAttribute : Attribute
    {
        public PercentageAttribute(double percent, double initial) => (Percent, Initial) = (percent, initial);
        public double Percent { get; private set; }
        public double Initial { get; private set; }
    }
}
