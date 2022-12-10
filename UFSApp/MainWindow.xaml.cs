using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

        private Sorter listViewSorter = null;
        private GridViewColumnHeader listViewSortCol = null;

        public ObservableCollection<FolderTreeViewItem> FolderTree { get; set; }

        public ObservableCollection<FileItem> FilesList { get; set; }

        private string rootPath;
        private string[] years;
        private string[] types;
        private string[] stages;

        public MainWindow()
        {
            FolderTree = new ObservableCollection<FolderTreeViewItem>();
            FilesList = new ObservableCollection<FileItem>();

            InitializeComponent();

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );

            LoadConfig();
            ScanDirectory();
        }

        private void filesViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (String.IsNullOrEmpty(fNameFilter.Text))
            {
                e.Accepted = true;
            }
            else
            {
                var fItem = (e.Item as FileItem);
                if (fItem != null && fItem.FInfo != null)
                {
                    e.Accepted = fItem.FInfo.Name.Contains(fNameFilter.Text, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    e.Accepted = true;
                }
            }
        }

        private void fNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lvFiles.ItemsSource != null)
            {
                var view = CollectionViewSource.GetDefaultView(lvFiles.ItemsSource);
                if (view != null)
                {
                    view.Refresh();
                }
            }
        }

        private void LoadConfig()
		{
			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

			var configuration = builder.Build();
			rootPath = configuration["RootPath"];
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = @"C:\Проверки";
            }

            var tmp = configuration["Years"];
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "2012|2013|2014|2015|2016";
			}

			years = tmp.Split(separator);

			tmp = configuration["ControlTypes"];
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "Мониторинг|ТЗ|ЦЗ";
			}

			types = tmp.Split(separator);

			tmp = configuration["Stages"];
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
                    var createdDir = Directory.CreateDirectory(rootPath);
                }

                LoadDirectories(rootPath, FolderTree, 1);
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
                LoadFiles(tag.FullName);
            }
        }

        private bool CheckDirLevel(string dirName, uint level)
        {
			switch (level)
            {
                case 1: //Год проверки
                    return years.Contains(dirName);
                case 2: //Тип проверки
                    return types.Contains(dirName);
                case 3: //Номер проверки
                    return uint.TryParse(dirName, out _);
                case 4: //Этап проверки
                    return stages.Contains(dirName);
                default:
                    return false;
            }
        }

        void LoadFiles(string path)
        {
            FilesList.Clear();
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                if ((fInfo.Attributes & FileAttributes.Hidden) > 0)
                {
                    continue;
                }
                var item = new FileItem() { FInfo = fInfo, Checked = false };
                FilesList.Add(item);
            }
        }

        private void cbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            bool? idChecked = cbSelectAll.IsChecked;
            if (idChecked.HasValue)
            {
                foreach (var item in lvFiles.Items)
                {
                    var file = item as FileItem;
                    if (file != null)
                    {
                        file.Checked = idChecked.Value;
                    }
                }
            }
        }

        private void lvFilesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSorter);
                lvFiles.Items.SortDescriptions.Clear();
            }

            var column = (sender as GridViewColumnHeader);
            if (column != null)
            {
                string sortBy = column.Tag.ToString();

                var newDir = ListSortDirection.Ascending;
                if (listViewSortCol == column && listViewSorter.Direction == newDir)
                {
                    newDir = ListSortDirection.Descending;
                }

                listViewSortCol = column;
                listViewSorter = new Sorter(listViewSortCol, newDir);
                AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSorter);
                lvFiles.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
            }
        }
    }
}
