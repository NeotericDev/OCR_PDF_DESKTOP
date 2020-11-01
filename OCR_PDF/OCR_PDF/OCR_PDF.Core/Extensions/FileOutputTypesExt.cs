using OCR_PDF.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OCR_PDF.Core
{
    public static class FileOutputTypesExt
    {
        public static string GetName(this OutputFileType outputFileType) => GetAttr<FileTypeAttribute, OutputFileType>(outputFileType).Name;

        public static string GetExtension(this OutputFileType outputFileType) => GetAttr<FileTypeAttribute, OutputFileType>(outputFileType).Extension;

        public static IEnumerable<OutputFileType> GetValues(this OutputFileType outputFileType)
        {
            yield return OutputFileType.PDF;
            yield return OutputFileType.TEXT;
        }

        public static double GetPercentage(this ProcessStage processStage) => GetAttr<PercentageAttribute, ProcessStage>(processStage).Percent;
        public static double GetInitialValue(this ProcessStage processStage) => GetAttr<PercentageAttribute, ProcessStage>(processStage).Initial;

        private static Tattr GetAttr<Tattr, Tenum>(Tenum p) where Tattr:Attribute
        {
            return (Tattr)Attribute.GetCustomAttribute(ForValue(p), typeof(Tattr));
        }

        private static MemberInfo ForValue<Tenum>(Tenum p)
        {
            return typeof(Tenum).GetField(Enum.GetName(typeof(Tenum), p));
        }
    }
}
