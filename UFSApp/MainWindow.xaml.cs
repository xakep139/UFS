using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UFSApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ScanDirectory();
        }

        void ScanDirectory()
        {
            string rootPath = ConfigurationManager.AppSettings["RootPath"];
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = @"C:\Проверки";
            }

            try
            {
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                LoadDirectories(rootPath, revisionsTreeView.Items);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadDirectories(string path, ItemCollection Items)
        {
            var directories = Directory.GetDirectories(path);
            foreach (var item in directories)
            {
                DirectoryInfo dInfo = new DirectoryInfo(item);
                var node = new CustomTreeViewItem() { Header = dInfo.Name, Tag = dInfo, Image = "" };
                var currentPath = Path.Combine(path, item);
                if (Directory.GetDirectories(currentPath).Length > 0)
                {
                    LoadDirectories(currentPath, node.Items);
                }
                else
                {
                    //LoadFiles(currentPath, node.Items);
                    var list = LoadFiles(currentPath);
                    if (list.Count > 0)
                    {
                        itemsListBox.DataContext = list;
                    }
                }
                Items.Add(node);
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

        List<Item> LoadFiles(string path)
        {
            List<Item> list = new List<Item>();
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                if (fInfo.Attributes == (fInfo.Attributes | FileAttributes.Hidden))
                {
                    continue;
                }
                var item = new Item() { Name = fInfo.Name, FInfo = fInfo };
                list.Add(item);
            }
            return list;
        }
    }
}
