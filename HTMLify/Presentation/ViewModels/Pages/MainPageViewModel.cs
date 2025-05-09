using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HTMLify.Application.Commands;
using HTMLify.Application.Services;
using HTMLify.Models.Pages;
using HTMLify.Presentation.ViewModels;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsAPICodePack.Dialogs;

namespace HTMLify.Presentation.ViewModels.Pages
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly List<string> _failedFiles = [];

        // 서비스 클래스를 App.xaml.cs에 등록해서 주입받아 사용
        private readonly AppSettingsService _appSettingsService;
        private readonly FileProccessingService _fileProccessingService;

        private readonly SelectInputFolderCommand _selectInputFolderCommand;
        private readonly SelectOutputFolderCommand _selectOutputFolderCommand;
        private readonly ConvertFilesCommand _convertFilesCommand;
        private readonly DownloadFailedFilesCommand _downloadFailedFilesCommand;

        public MainPageViewModel(
            AppSettingsService appSettingsService,
            FileProccessingService fileProccessingService
        )
        {
            _appSettingsService = appSettingsService;
            _fileProccessingService = fileProccessingService;

            LoadSettings();

            _selectInputFolderCommand = new SelectInputFolderCommand(this);
            _selectOutputFolderCommand = new SelectOutputFolderCommand(this);
            _convertFilesCommand = new ConvertFilesCommand(this);
            _downloadFailedFilesCommand = new DownloadFailedFilesCommand(this);
        } // constructor

        #region Observable Properties
        [ObservableProperty] private string _inputFolder = "C:\\mht_files";
        [ObservableProperty] private string _outputFolder = "C:\\html_files";
        [ObservableProperty] private string _log = string.Empty;
        [ObservableProperty] private double _progress;
        [ObservableProperty] private int _successCount;
        [ObservableProperty] private int _failureCount;
        #endregion

        #region RelayCommands
        public ICommand SelectInputFolderCommand => _selectInputFolderCommand;
        public ICommand SelectOutputFolderCommand => _selectOutputFolderCommand;
        public ICommand ConvertFilesCommand => _convertFilesCommand;
        public ICommand DownloadFailedFilesCommand => _downloadFailedFilesCommand;
        #endregion

        #region Method
        public async Task ConvertFiles()
        {
            SuccessCount = 0;
            FailureCount = 0;
            _failedFiles.Clear();

            if (!Directory.Exists(InputFolder))
            {
                Log += "\n입력 폴더를 찾을 수 없습니다.";
                return;
            }

            if (!Directory.Exists(OutputFolder))
            {
                Log += "\n출력 폴더를 찾을 수 없습니다.";
                return;
            }

            var allFiles = Directory.GetFiles(InputFolder, "*", SearchOption.AllDirectories);
            if (allFiles.Length == 0)
            {
                Log += "\n입력 폴더에 파일이 없습니다.";
                return;
            }

            Log += $"\n총 {allFiles.Length}개의 파일을 처리합니다.";
            int successCount = 0;
            int copyCount = 0;

            await Task.Run(() =>
            {
                var logBuffer = new List<string>();

                for (int i = 0; i < allFiles.Length; i++)
                {
                    var file = allFiles[i];
                    try
                    {
                        string relativePath = file[(InputFolder.Length + 1)..];
                        string outputFilePath = string.Empty;

                        if (Path.GetExtension(file).Equals(".mht", StringComparison.OrdinalIgnoreCase))
                        {
                            string newFileName = Path.GetFileNameWithoutExtension(relativePath) + ".html";
                            string newDirPath = Path.Combine(OutputFolder, Path.GetDirectoryName(relativePath) ?? "");
                            outputFilePath = Path.Combine(newDirPath, newFileName);

                            if (!Directory.Exists(newDirPath))
                            {
                                Directory.CreateDirectory(newDirPath);
                            }

                            _fileProccessingService.ConvertMhtToHtml(file, outputFilePath);
                            logBuffer.Add($"변환 완료: {outputFilePath}");
                            successCount++;
                        }
                        else
                        {
                            outputFilePath = Path.Combine(OutputFolder, relativePath);
                            string outputDir = Path.GetDirectoryName(outputFilePath);

                            if (!Directory.Exists(outputDir))
                            {
                                Directory.CreateDirectory(outputDir);
                            }

                            File.Copy(file, outputFilePath, true);
                            logBuffer.Add($"복사 완료: {outputFilePath}");
                            copyCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        logBuffer.Add($"오류 발생 ({file}): {ex.Message}");
                        _failedFiles.Add(file);
                        FailureCount++;
                    }

                    // 로그 버퍼를 일정 주기로 UI에 업데이트
                    if (logBuffer.Count >= 10 || i == allFiles.Length - 1)
                    {
                        var batch = string.Join("\n", logBuffer);
                        AppendLog(batch);
                        logBuffer.Clear();
                    }

                    UpdateProgress((double)(i + 1) / allFiles.Length);
                }
            });

            SuccessCount = successCount + copyCount;
            Log += $"\n변환 완료, MHT 변환: {successCount}개, 일반 파일 복사: {copyCount}개.";
        } // ConvertFiles()

        public void DownloadFailedFiles()
        {
            if (_failedFiles.Count == 0)
            {
                Log += "\n실패한 파일이 없습니다.";
                return;
            }

            var savePath = Path.Combine(OutputFolder, "FailedFiles.txt");
            File.WriteAllLines(savePath, _failedFiles);
            Log += $"\n실패 파일 목록 저장 완료: {savePath}";
        }

        public void SaveSetting()
        {
            try
            {
                var settings = new AppSettings
                {
                    InputFolder = InputFolder,
                    OutputFolder = OutputFolder
                };
                _appSettingsService.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                Log += $"\n설정 저장 오류: {ex.Message}";
            }
        }

        private void LoadSettings()
        {
            var settings = _appSettingsService.LoadSettings();
            if (settings is not null)
            {
                InputFolder = settings.InputFolder;
                OutputFolder = settings.OutputFolder;
            }
        }

        private void UpdateProgress(double value)
        {
            App.Current.Dispatcher.Invoke(() => Progress = value);
        }
        private void AppendLog(string message)
        {
            App.Current.Dispatcher.BeginInvoke(() => Log += "\n" + message);
        }
        #endregion

    }
}
