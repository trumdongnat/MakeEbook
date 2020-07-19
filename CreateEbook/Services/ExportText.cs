using CreateEbook.Helpers;
using CreateEbook.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CreateEbook.Services
{
    public class ExportText
    {
        private const string BOOK_SLOT = "[BOOK]";
        private const string VOL_SLOT = "[VOL]";
        private const string AUTHOR_SLOT = "[AUTHOR]";
        private const string CHAPTER_SLOT = "[CHAPTER]";
        private const string PARAGRAPH_SLOT = "[PARAGRAPH]";
        private const string ID_SLOT = "[ID]";
        private const string SECTION_MARK = "[*]";

        private string _chapterTemplate;
        private Ebook _ebook;
        private string _folder;

        public ExportText(Ebook ebook, string folder)
        {
            _ebook = ebook;
            _folder = folder;
            _chapterTemplate = File.ReadAllText(AssetHelper.ChapterTemplatePath);
        }

        public void Export()
        {
            if (_ebook.HasDescription)
            {
                ExportDescripiton(_ebook.Description);
            }
            foreach (var chapter in _ebook.Chapters)
            {
                ExportChapter(chapter);
            }
            ExportTOC();
            
        }

        private void ExportDescripiton(string description)
        {
            //build text
            var sections = _chapterTemplate.Split(new string[] { SECTION_MARK }, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            foreach (var section in sections)
            {
                var text = section.Replace(BOOK_SLOT, _ebook.Name)
                    .Replace(AUTHOR_SLOT, _ebook.Author)
                    .Replace(VOL_SLOT, string.Empty)
                    .Replace(CHAPTER_SLOT, "Giới thiệu")
                    .Replace(ID_SLOT, $"description");
                if (text.Contains(PARAGRAPH_SLOT))
                {
                    string[] lines = description.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                    foreach (var line in lines)
                    {
                        builder.Append("    ").AppendLine(text.Replace(PARAGRAPH_SLOT, line));
                    }
                }
                else
                {
                    builder.Append(text);
                }
            }
            //write to file
            var fileName = $"description.html";
            var filePath = Path.Combine(_folder, fileName);
            Debug.WriteLine($"Export description to {filePath}");
            var content = builder.ToString();
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        private void ExportTOC()
        {
            var template = File.ReadAllText(AssetHelper.TocHtmlPath);
            var builder = new StringBuilder();
            if (_ebook.HasDescription)
            {
                var tag = $"<div class=\"lv2\"><a href = \"description.html\" >Giới thiệu</a></div>";
                builder.AppendLine(tag);
            }
            foreach (var chapter in _ebook.Chapters)
            {
                var tag = $"<div class=\"lv2\"><a href = \"C{chapter.Index}.html\" >{chapter.Name}</a></div>";
                builder.AppendLine(tag);
            }
            var text = builder.ToString();
            template = template.Replace("[TOC]", text);
            var filePath = Path.Combine(_folder, "mucluc.html");
            File.WriteAllText(filePath, template);
        }

        private void ExportChapter(Chapter chapter)
        {
            //build text
            var sections = _chapterTemplate.Split(new string[] { SECTION_MARK }, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            foreach (var section in sections)
            {
                var text = section.Replace(BOOK_SLOT, _ebook.Name)
                    .Replace(AUTHOR_SLOT, _ebook.Author)
                    .Replace(VOL_SLOT, chapter.VolName)
                    .Replace(CHAPTER_SLOT, chapter.Name)
                    .Replace(ID_SLOT, $"C{chapter.Index}");
                if (text.Contains(PARAGRAPH_SLOT))
                {
                    string[] lines = chapter.Text.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                    foreach (var line in lines)
                    {
                        builder.Append("    ").AppendLine(text.Replace(PARAGRAPH_SLOT, line));
                    }
                }
                else
                {
                    builder.Append(text);
                }
            }
            //write to file
            var fileName = $"C{chapter.Index}.html";
            var filePath = Path.Combine(_folder, fileName);
            Debug.WriteLine($"Export chapter {chapter.Index} to {filePath}");
            var content = builder.ToString();
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}