using HTMLify.Presentation.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsAPICodePack.Dialogs;

namespace HTMLify.Application.Commands
{
    public class SelectInputFolderCommand(MainPageViewModel viewModel) : ICommand
    {
        private readonly MainPageViewModel _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Directory.Exists(_viewModel.InputFolder) ?
                    _viewModel.InputFolder : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() is CommonFileDialogResult.Ok)
            {
                _viewModel.InputFolder = dialog.FileName;
                _viewModel.SaveSetting();
            }
        } // Execute()

    }
}
