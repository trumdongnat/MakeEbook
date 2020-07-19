using MakeEbook.Helpers;
using MakeEbook.Models;
using MakeEbook.Properties;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace MakeEbook.Services
{
    public class ExportService
    {
        public string CompressLevel { get; set; } = "-c1";

        public void Export(Ebook ebook, string workspace)
        {
            //prepare
            ebook.PrepareToExport();
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