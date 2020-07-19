using MakeEbook.Helpers;

namespace MakeEbook.Models
{
    public class Chapter :BindableBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _volName;
        public string VolumnName
        {
            get => _volName;
            set => SetProperty(ref _volName, value);
        }
        private int _index;
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}