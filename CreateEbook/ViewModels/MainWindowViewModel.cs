using CreateEbook.Helpers;
using CreateEbook.Models;
using CreateEbook.Properties;
using CreateEbook.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CreateEbook.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ImportService ImportService { get; set; } = new ImportService();
        public ExportService ExportService { get; set; } = new ExportService();

        private Ebook _ebook;

        public Ebook Ebook
        {
            get => _ebook;
            set => SetProperty(ref _ebook, value);
        }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private string _workspace;

        public string Workspace
        {
            get => _workspace;
            set => SetProperty(ref _workspace, value);
        }

        private string _exportProgress;

        public string ExportProgress
        {
            get => _exportProgress;
            set => SetProperty(ref _exportProgress, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _toolPath = Settings.Default.ToolPath;

        public string ToolPath
        {
            get => _toolPath;
            set
            {
                SetProperty(ref _toolPath, value);
                Settings.Default.ToolPath = value;
                Settings.Default.Save();
            }
        }

        public ICommand SelectFileCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand SelectCoverCommand { get; set; }
        public ICommand RemoveCoverCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        public ICommand SelectToolCommand { get; set; }

        public MainWindowViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            ImportCommand = new RelayCommand(Import, CanImport);
            SelectCoverCommand = new RelayCommand(SelectCover);
            RemoveCoverCommand = new RelayCommand(RemoveCover);
            ExportCommand = new RelayCommand(Export);
            SelectToolCommand = new RelayCommand(SelectTool);
        }

        private void SelectTool()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Kindle Gen (*.exe)|*.exe";
            if (fileDialog.ShowDialog() == true)
            {
                ToolPath = fileDialog.FileName;
            }
        }

        private void SelectFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                FilePath = fileDialog.FileName;
            }
        }

        private bool CanImport()
        {
            return !_isImporting;
        }

        private void Import()
        {
            ImportAsync();
        }

        private bool _isImporting;
        private CancellationTokenSource _importCts;

        private async void ImportAsync()
        {
            IsBusy = true;
            _importCts = new CancellationTokenSource();
            try
            {
                _isImporting = true;
                ImportCommand.CanExecute(null);
                Ebook = await ImportService.ImportAsync(FilePath, _importCts.Token);
                _isImporting = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            IsBusy = false;
        }

        private void SelectCover()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Images|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                var uri = new Uri(fileDialog.FileName);
                Ebook.Cover = new System.Windows.Media.Imaging.BitmapImage(uri);
            }
        }

        private void RemoveCover()
        {
            Ebook.Cover = null;
        }

        private async void Export()
        {
            if (!File.Exists(ToolPath))
            {
                MessageBox.Show("Không tìm thấy kindle gen tool", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(Ebook.Author))
            {
                MessageBox.Show("Không để trống tên tác giả", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(Ebook.Name))
            {
                MessageBox.Show("Không để trống tên sách", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            IsBusy = true;
            var path = GetWorkspace(Ebook.Name);
            if (File.Exists(path))
            {
                Directory.Delete(path);
            }
            Directory.CreateDirectory(path);
            await Task.Run(() => ExportService.Export(Ebook, path));
            IsBusy = false;
        }

        private string GetWorkspace(string name)
        {
            var folderName = TextHelper.ConvertToUnsign(name);
            folderName = TextHelper.FixFolderName(folderName);
            var path = Path.Combine(AssetHelper.OuputFolder, folderName);
            return path;
        }
    }
}