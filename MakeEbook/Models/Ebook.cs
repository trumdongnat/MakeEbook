using MakeEbook.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace MakeEbook.Models
{
    public class Ebook : BindableBase, IDataErrorInfo
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _author;

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        private BitmapImage _cover;

        public BitmapImage Cover
        {
            get => _cover;
            set => SetProperty(ref _cover, value);
        }

        public List<Chapter> Chapters { get; set; }

        //private bool _isAddDescription;
        //public bool IsAddDescription
        //{
        //    get => _isAddDescription;
        //    set => SetProperty(ref _isAddDescription, value);
        //}

        public bool HasDescription => !string.IsNullOrWhiteSpace(_description);

        private string _description;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _isAutoSplitVol;

        public bool IsAutoSplitVol
        {
            get => _isAutoSplitVol;
            set => SetProperty(ref _isAutoSplitVol, value);
        }

        private int _autoSplitInterval = 100;

        public int AutoSplitInterval
        {
            get => _autoSplitInterval;
            set => SetProperty(ref _autoSplitInterval, value);
        }

        public string Charset = "UTF-8";
        public bool HasVolumn => Volumns != null && Volumns.Count > 0;

        public List<Volumn> Volumns { get; set; }

        public void PrepareToExport()
        {
            Volumns = null;
            if (IsAutoSplitVol)
            {
                SplitVolumn();
            }

            if (!string.IsNullOrWhiteSpace(Chapters[0].VolumnName))
            {
                Volumns = new List<Volumn>();
                var groups = Chapters.GroupBy(x => x.VolumnName);
                int index = 0;
                foreach (var group in groups)
                {
                    index++;
                    var volumn = new Volumn
                    {
                        Index = index,
                        Name = group.Key,
                        Chapters = group.ToList()
                    };
                    Volumns.Add(volumn);
                }
            }
        }

        private void SplitVolumn()
        {
            int count = 0;
            string volumnName = null;
            for (int i = 0; i < Chapters.Count; i++)
            {
                var chapter = Chapters[i];
                if (count == 0)
                {
                    var start = i;
                    var end = i + AutoSplitInterval -1;
                    if (end < Chapters.Count - 1)
                    {
                        volumnName = $"Chương {Chapters[start].Index } - {Chapters[end].Index }";
                    }
                    else
                    {
                        volumnName = $"Chương {Chapters[start].Index } - {Chapters[Chapters.Count - 1].Index } (HẾT)";
                    }
                }
                chapter.VolumnName = volumnName;
                count++;
                if (count == AutoSplitInterval)
                {
                    count = 0;
                }
            }
           
        }

        #region IDataErrorInfo

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Name))
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return "Không bỏ trống tên";
                    }
                }
                else if (columnName == nameof(Author))
                {
                    return "Không bỏ trống tên tác giả";
                }
                return null;
            }
        }

        #endregion IDataErrorInfo
    }
}