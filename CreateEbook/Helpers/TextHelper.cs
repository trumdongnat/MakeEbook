using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CreateEbook.Helpers
{
    public class TextHelper
    {
        public static string ConvertToUnsign(string text)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = text.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string FixFolderName(string name)
        {
            var fixedName = Regex.Replace(name, "[^a-zA-Z0-9_]", "_");
            return fixedName.Replace("\"", "").Trim();
        }

        public static string FixFileName(string name)
        {
            var result = name;
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                result = result.Replace(invalidChar, '_');
            }
            return result;
        }
    }
}