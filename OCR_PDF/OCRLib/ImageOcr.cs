using Syncfusion.OCRProcessor;
using System;
using System.Drawing;

namespace OCRLib
{
    public class ImageOcr
    {
        private OCRProcessor oCRProcessor;
        private readonly string tessDataPath;

        public ImageOcr(string tessBinaries, string tessDataPath)
        {
            this.tessDataPath = tessDataPath;
            oCRProcessor = new OCRProcessor(@"C:\Users\Dev\Downloads\syncfusionocrprocessor\Tesseractbinaries_core\Windows");
            OCRSettings settings = new OCRSettings();
            
            //oCRProcessor.Settings.Te = TesseractVersion.Version4_0;
        }
         
        public string DoOCR(Bitmap bitmap)
        {
            oCRProcessor.Settings.Language = Languages.English;
            return oCRProcessor.PerformOCR(bitmap, tessDataPath);
        }
    }
}
