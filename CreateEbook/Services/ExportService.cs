using CreateEbook.Helpers;
using CreateEbook.Models;
using CreateEbook.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace CreateEbook.Services
{
    public class ExportService
    {
        public string CompressLevel { get; set; } = "-c1";

        public void Export(Ebook ebook, string workspace)
        {
            //prepare
            //Prepare(ebook);
            //export
            var dataPath = Path.Combine(workspace, "data");
            Directory.CreateDirectory(dataPath);
            var textPath = Path.Combine(dataPath, "Text");
            Directory.CreateDirectory(textPath);
            ExportMisc(dataPath);
            ExportCover(ebook, dataPath);
            ExportText(ebook, textPath);
            ExportToc(ebook, dataPath);
            RunCommand(workspace, dataPath, ebook);
        }

        private void Prepare(Ebook ebook)
        {
            if (ebook.IsAutoSplitVol && string.IsNullOrWhiteSpace(ebook.Chapters[0].VolName))
            {
                int count = 0;
                string volName = null;
                foreach (var chapter in ebook.Chapters)
                {
                    if(count == 0)
                    {
                        var start = chapter.Index;
                        var end = chapter.Index + ebook.AutoSplitInterval;
                        if(end < ebook.Chapters.Count - 1)
                        {
                            volName = $"Chương {start + 1} - {end + 1}";
                        }
                        else
                        {
                            volName = $"Chương {start + 1} - {ebook.Chapters.Count - 1 } (HẾT)";
                        }
                    }
                    chapter.VolName = volName;
                    count++;
                    if(count == ebook.AutoSplitInterval)
                    {
                        count = 0;
                    }
                }
            }
        }

        private void ExportMisc(string folder)
        {
            var filePath = Path.Combine(folder, "stylesheet.css");
            File.Copy(AssetHelper.StyleSheetPath, filePath, true);
        }

        private void ExportCover(Ebook ebook, string folder)
        {
            if (ebook.Cover != null)
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ebook.Cover));

                var filePath = Path.Combine(folder, "cover.jpg");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private void ExportText(Ebook ebook, string folder)
        {
            var exportText = new ExportText(ebook, folder);
            exportText.Export();
        }

        private void ExportToc(Ebook ebook, string dataPath)
        {
            var exportTocService = new ExportToc(ebook, dataPath);
            exportTocService.Export();
        }

        private void RunCommand(string workspace, string dataPath, Ebook ebook)
        {
            
            var contentOpfPath = Path.Combine(dataPath, "content.opf");
            var command = $"/C \"\"{Settings.Default.ToolPath}\" \"{contentOpfPath}\" {CompressLevel}\"";
            
            //run command
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            var process = Process.Start(startInfo);
            process.WaitForExit();

            //copy output
            var fileName = $"{ebook.Name}.mobi";
            fileName = TextHelper.FixFileName(fileName);
            var filePath = Path.Combine(workspace, fileName);
            File.Move(Path.Combine(dataPath, "content.mobi"), filePath);
        }
    }
}