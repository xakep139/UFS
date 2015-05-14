using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UFSApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const uint maxLevel = 4;
        private const char separator = '|';

        private string rootPath;
        private string[] years;
        private string[] types;
        private string[] stages;

        private ObservableCollection<FolderTreeViewItem> items = new ObservableCollection<FolderTreeViewItem>();

        public MainWindow()
        {
            InitializeComponent();

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );

            LoadConfig();
            ScanDirectory();
            revisionsTreeView.DataContext = items;
        }

        private void LoadConfig()
        {
            rootPath = ConfigurationManager.AppSettings["RootPath"];
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = @"C:\Проверки";
            }

            var tmp = ConfigurationManager.AppSettings["Years"];
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "2012|2013|2014|2015|2016";
            }
            years = tmp.Split(separator);

            tmp = ConfigurationManager.AppSettings["ControlTypes"];
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "Мониторинг|ТЗ|ЦЗ";
            }
            types = tmp.Split(separator);

            tmp = ConfigurationManager.AppSettings["Stages"];
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "1. Подготовка|2. Проведение|3. Итоги|4. Реализация|5. Контроль";
            }
            stages = tmp.Split(separator);
        }

        void ScanDirectory()
        {
            try
            {
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                LoadDirectories(rootPath, items, 1);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadDirectories(string path, IList Items, uint level)
        {
            var directories = Directory.GetDirectories(path);
            foreach (var curDir in directories)
            {
                var dInfo = new DirectoryInfo(curDir);
                if (!CheckDirLevel(dInfo.Name, level))
                {
                    continue;
                }
                var node = new FolderTreeViewItem() { Header = dInfo.Name, Tag = dInfo };
                if (level < maxLevel)
                {
                    LoadDirectories(curDir, node.Items, level + 1);
                }
                else    //Каталоги этапов проверки
                {
                    node.Selected += node_Selected;
                }
                Items.Add(node);
            }
        }

        void node_Selected(object sender, RoutedEventArgs e)
        {
            var node = sender as FolderTreeViewItem;
            if (node != null && node.Tag as DirectoryInfo != null)
            {
                var tag = node.Tag as DirectoryInfo;
                var list = LoadFiles(tag.FullName);
                itemsListBox.DataContext = list;
            }
        }

        private bool CheckDirLevel(string dirName, uint level)
        {
            uint tmp;
            switch (level)
            {
                case 1: //Год проверки
                    return years.Contains(dirName);
                case 2: //Тип проверки
                    return types.Contains(dirName);
                case 3: //Номер проверки
                    return uint.TryParse(dirName, out tmp);
                case 4: //Этап проверки
                    return stages.Contains(dirName);
                default:
                    return false;
            }
        }

        void LoadFiles(string path, ItemCollection Items)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                var node = new TreeViewItem() { Header = fInfo.Name, Tag = fInfo };
                Items.Add(node);
            }
        }

        List<FileItem> LoadFiles(string path)
        {
            var list = new List<FileItem>();
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                if ((fInfo.Attributes & FileAttributes.Hidden) > 0)
                {
                    continue;
                }
                var item = new FileItem() { Name = fInfo.Name, FInfo = fInfo };
                list.Add(item);
            }
            return list;
        }
    }
}
