using MakeEbook.Models;
using MakeEbook.Services;
using MakeEbook.ViewModels;
using MakeEbook.Views;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MakeEbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        private MainWindowViewModel _viewModel = new MainWindowViewModel();
        public MainWindow()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var element = sender as FrameworkElement;
                var chapter = element.DataContext as Chapter;
                ShowChapter(chapter);
            }
        }

        private void ShowChapter(Chapter chapter)
        {
            Window window = new MetroWindow();
            window.Content = new ChapterView(chapter);
            window.ShowDialog();
        }

        private void ButtonEditDescription_Click(object sender, RoutedEventArgs e)
        {
            Window window = new MetroWindow();
            window.Content = new DescriptionView(_viewModel.Ebook);
            window.ShowDialog();
        }
    }
}
