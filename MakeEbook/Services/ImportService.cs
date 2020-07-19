using MakeEbook.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MakeEbook.Services
{
    public class ImportService
    {
        public string ChapterNamePattern { get; set; } = @"Chương \d+:? .*";
        public string VolNamePattern { get; set; } = @"Quyển \d+:? .*";
        public bool IsIgnoreEmptyLine { get; set; } = true;
        public bool IsTrimLine { get; set; } = true;

        public Ebook Import(string path, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File not found!");
            }
            string description = null;
            var chapterIndex = 0;
            var chapters = new List<Chapter>();
            Chapter chapter = null;//first chapter is description
            StringBuilder chapterText = new StringBuilder();
            string volumn = null;
            string line;
            StreamReader file = new StreamReader(path);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (IsTrimLine)
                    {
                        line = line.Trim();
                    }
                    if (IsIgnoreEmptyLine && string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    if(Regex.IsMatch(line, VolNamePattern))//new volumn
                    {
                        volumn = line;
                    }
                    if (Regex.IsMatch(line, ChapterNamePattern)) // new chapter
                    {
                        //remove last new line
                        var text = chapterText.ToString();
                        if (text.EndsWith(Environment.NewLine))
                        {
                            text = text.Substring(0, text.Length - Environment.NewLine.Length);
                        }
                        //save current chapter
                        if(chapter == null)//description
                        {
                            description = text;
                        }
                        else
                        {
                            chapter.Text = text;
                            chapters.Add(chapter);
                        }
                        
                        //make new chapter
                        chapter = new Chapter() { Index = ++chapterIndex };
                        chapter.Name = line;
                        chapter.VolumnName = volumn;
                        chapterText = new StringBuilder();
                    }
                    else
                    {
                        chapterText.AppendLine(line);
                    }
                }
                //last chapter
                chapter.Text = chapterText.ToString();
                chapters.Add(chapter);

                file.Close();

                return new Ebook
                {
                    Chapters = chapters,
                    Name = Path.GetFileNameWithoutExtension(path),
                    Description = description
                };
            }
            catch (Exception)
            {
                file.Close();
                throw;
            }
        }

        public Task<Ebook> ImportAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Import(path, cancellationToken));
        }
    }
}