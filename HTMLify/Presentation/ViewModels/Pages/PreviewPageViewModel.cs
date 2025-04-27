using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HTMLify.Models.Pages;
using HTMLify.Presentation.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HTMLify.Presentation.ViewModels.Pages
{
    public partial class PreviewPageViewModel : BaseViewModel
    {
        private const string _FileStoragePath = "fileList.json";

        [ObservableProperty]
        private ObservableCollection<FileItem> _files = [];

        [ObservableProperty]
        private FileItem? _selectedFile;

        [ObservableProperty]
        public Uri? _selectedFilePath;

        public PreviewPageViewModel()
        {
            LoadFiles();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName is nameof(SelectedFile))
                {
                    removeFileCommand?.NotifyCanExecuteChanged();
                }

                if (e.PropertyName is nameof(SelectedFile))
                {
                    if (SelectedFile is not null)
                    {
                        SelectedFilePath = new Uri($"file:///{SelectedFile.FullPath.Replace("\\", "/")}");
                    }
                    else
                    {
                        _selectedFilePath = null;
                    }
                }
            };
        } // default constructor

        [RelayCommand]
        private void AddFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "웹 파일 (*.mht;*.html)|*.mht;*.html"
            };

            if (dialog.ShowDialog() is true)
            {
                foreach (var file in dialog.FileNames)
                {
                    if (!Files.Any(f => f.FullPath == file))
                    {
                        Files.Add(new FileItem
                        {
                            FileName = Path.GetFileName(file),
                            FullPath = file
                        });
                    }
                }

                SaveFiles();
            }
        } // AddFiles()

        [RelayCommand(CanExecute = nameof(CanRemoveFile))]
        private void RemoveFile()
        {
            if (SelectedFile is not null)
            {
                Files.Remove(SelectedFile);
                SelectedFile = null;
                SaveFiles();
            }
        } // RemoveFiles()

        [RelayCommand(CanExecute = nameof(CanRemoveFile))]
        private void RemoveAllFiles()
        {
            Files.Clear();
            SelectedFile = null;
            SaveFiles();
        } // RemoveFiles()

        private void SaveFiles()
        {
            var json = JsonSerializer.Serialize(Files);
            File.WriteAllText(_FileStoragePath, json);
        }

        private void LoadFiles()
        {
            if (File.Exists(_FileStoragePath))
            {
                var json = File.ReadAllText(_FileStoragePath);
                var fileItems = JsonSerializer.Deserialize<ObservableCollection<FileItem>>(json);

                if (fileItems is not null)
                {
                    Files = new ObservableCollection<FileItem>(fileItems);
                }    
            }
        }

        private bool CanRemoveFile()
        {
            if (SelectedFile is null) return false; // 선택된 파일이 없을 때 삭제 불가능
            else return true; // 선택된 파일이 있을 때 삭제 가능
        }
    }
}
