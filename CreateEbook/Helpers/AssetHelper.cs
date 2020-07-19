using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CreateEbook.Helpers
{
    public static class AssetHelper
    {
        public static string AppFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static string AssetFolder = Path.Combine(AppFolder, "Assets");
        public static string ChapterTemplatePath = Path.Combine(AssetFolder, @"chapter.html");
        public static string StyleSheetPath = Path.Combine(AssetFolder, @"stylesheet.css");
        public static string ContentOpfPath = Path.Combine(AssetFolder, @"content.opf");
        public static string TocNcxPath = Path.Combine(AssetFolder, @"toc.ncx");
        public static string TocHtmlPath = Path.Combine(AssetFolder, @"mucluc.html");
        public static string OuputFolder = Path.Combine(AppFolder, @"Output");
    }
}
