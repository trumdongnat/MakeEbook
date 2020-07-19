using CreateEbook.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace CreateEbook.Models
{
    public class Ebook:BindableBase
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
    }
}