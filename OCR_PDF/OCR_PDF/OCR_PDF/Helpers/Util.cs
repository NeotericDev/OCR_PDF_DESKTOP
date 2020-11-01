using OCR_PDF.Core;
using OCR_PDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows;

namespace OCR_PDF.Helpers
{
    public class Util
    {
        public static IEnumerable<OutputFormatViewModel> GetAvailableOutputFormatsVMs() 
            => Constants.OUTPUT_FILE_TYPES.Select(fileType => new OutputFormatViewModel(fileType));

        public static IEnumerable<LanguageViewModel> GetAvailableInputLanguagesVMs(string tessDataPath)
            => Directory.GetFiles(tessDataPath, "")
            .Select(fileFullPath => fileFullPath.Substring(fileFullPath.LastIndexOf('\\') + 1))
            .Where(fileName => Constants.TESS_DATA_FILE_EXTENSION.Equals(fileName.Substring(fileName.LastIndexOf('.') + 1), StringComparison.InvariantCultureIgnoreCase))
            .Select(fileName => new LanguageViewModel(fileName.Substring(0, fileName.IndexOf('.'))));

        public static InputFileType GetInputFileType(FileInfo fileInfo) => fileInfo.Extension.IndexOf("pdf") > -1 ? InputFileType.PDF : InputFileType.IMAGE;

        //
        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            
            return bmp;
        }

        public static double CalculateProgress(ProcessStage stage, int completedPages, int totalPages) => stage.GetInitialValue() + ( completedPages / (double)totalPages * stage.GetPercentage() );

        public static ProcessStage GetImgOcrStage(InputFileType iFileType) => iFileType == InputFileType.PDF ? ProcessStage.PDF_IMG_OCR : ProcessStage.IMG_OCR;

        public static ProcessStage GetMergingrStage(InputFileType iFileType) => iFileType == InputFileType.PDF ? ProcessStage.PDF_TXT_PDF_MERGING : ProcessStage.IMG_TXT_PDF_MERGING;

        private FileInfo BitmapSourceToFile(BitmapSource bitmapsource, string saveAs)
        {
            BitmapFromSource(bitmapsource, File.OpenWrite(saveAs));
            return new FileInfo(saveAs);
        }
        private Bitmap BitmapSourceToFileBitmap(BitmapSource bitmapsource, string saveAs)
        {
            return BitmapFromSource(bitmapsource, File.OpenWrite(saveAs));
        }
        private Bitmap BitmapSourceToMemoryBitmap(BitmapSource bitmapsource)
        {
            return BitmapFromSource(bitmapsource, new MemoryStream());
        }

        private Bitmap BitmapFromSource(BitmapSource bitmapsource, Stream stream)
        {
            Bitmap bitmap;
            using (stream)
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(stream);
                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }

        public static string GetAppStartupPath() => System.AppDomain.CurrentDomain.BaseDirectory;

        public static string GetTessBinariesPath(string arch) =>  Path.Combine(GetAppStartupPath(), "TesseractBinaries", "x64".Equals(arch, StringComparison.InvariantCultureIgnoreCase) ? "Arch64" : "Arch86");

        public static string GetTessBinariesPath() => GetTessBinariesPath("x64");
    }
}
