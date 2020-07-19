using CreateEbook.Helpers;
using CreateEbook.Models;
using System;
using System.IO;
using System.Text;

namespace CreateEbook.Services
{
    internal class ExportToc
    {
        private Ebook _ebook;
        private string _folder;

        public ExportToc(Ebook ebook, string folder)
        {
            _ebook = ebook;
            _folder = folder;
        }

        public void Export()
        {
            ExportContentOpf();
            ExportTocNcx();
        }

        #region content.opf

        private void ExportContentOpf()
        {
            var content = MakeContentOpf();
            var path = Path.Combine(_folder, "content.opf");
            File.WriteAllText(path, content);
        }

        private string MakeContentOpf()
        {
            var text = File.ReadAllText(AssetHelper.ContentOpfPath);
            text = text.Replace("[NAME]", _ebook.Name)
                   .Replace("[AUTHOR]", _ebook.Author);
            var date = DateTime.Now.ToString("dd-MM-yyyy");
            text = text.Replace("[DATE]", date);
            var manifest = MakeManifest();
            text = text.Replace("[MANIFEST]", manifest);
            text = text.Replace("[IMAGE]", MakeImage());
            var ncx = MakeNcx();
            text = text.Replace("[NCX]", ncx);
            return text;
        }

        private string MakeNcx()
        {
            var builder = new StringBuilder();
            if (_ebook.HasDescription)
            {
                builder.AppendLine("    <itemref idref=\"description\"/>");
            }
            builder.AppendLine("    <itemref idref=\"mucluc\"/>");

            foreach (var chapter in _ebook.Chapters)
            {
                builder.AppendLine($"    <itemref idref=\"C{chapter.Index}\"/>");
            }
            return builder.ToString();
        }

        private string MakeManifest()
        {
            var builder = new StringBuilder();
            if (_ebook.HasDescription)
            {
                builder.AppendLine("    <item id=\"description\" href=\"Text/Description.html\" media-type=\"application/xhtml+xml\"/>");
            }
            builder.AppendLine("    <item id=\"mucluc\" href=\"Text/mucluc.html\" media-type=\"application/xhtml+xml\"/>");

            foreach (var chapter in _ebook.Chapters)
            {
                builder.AppendLine($"    <item id=\"C{chapter.Index}\" href=\"Text/C{chapter.Index}.html\" media-type=\"application/xhtml+xml\"/>");
            }
            return builder.ToString();
        }

        private string MakeImage()
        {
            //TODO
            return Environment.NewLine;
        }

        #endregion content.opf

        #region toc.ncx

        private void ExportTocNcx()
        {
            var toc = MakeToc();
            var path = Path.Combine(_folder, "toc.ncx");
            File.WriteAllText(path, toc);
        }

        private string MakeToc()
        {
            var text = File.ReadAllText(AssetHelper.TocNcxPath);
            text = text.Replace("[NAME]", _ebook.Name)
                   .Replace("[AUTHOR]", _ebook.Author);
            var builder = new StringBuilder();
            var index = 1;
            if (_ebook.HasDescription)
            {
                builder.AppendLine($@"    <navPoint id=""description"" playorder=""{index}"">");
                builder.AppendLine(@"       <navLabel>");
                builder.AppendLine($@"          <text>Giới thiệu</text>");
                builder.AppendLine(@"       </navLabel>");
                builder.AppendLine($@"       <content src=""Text/Desciption.html""/>");
                builder.AppendLine(@"    </navPoint>");
                index++;
            }
            foreach (var chapter in _ebook.Chapters)
            {
                builder.AppendLine($@"    <navPoint id=""nav{index}"" playorder=""{index}"">");
                builder.AppendLine(@"       <navLabel>");
                builder.AppendLine($@"          <text>{chapter.Name}</text>");
                builder.AppendLine(@"       </navLabel>");
                builder.AppendLine($@"       <content src=""Text/C{index}.html""/>");
                builder.AppendLine(@"    </navPoint>");
                index++;
            }
            text = text.Replace("[NAVPOINT]", builder.ToString());
            return text;
        }

        #endregion toc.ncx
    }
}