using System.Windows.Input;

namespace HTMLify.Application.Commands
{
    public class DownloadFailedFilesCommand(MainPageViewModel viewModel) : ICommand
    {
        private readonly MainPageViewModel _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _viewModel.DownloadFailedFiles();
        }
    }
}
