using System.Windows.Input;

namespace HTMLify.Application.Commands
{
    public class ConvertFilesCommand(MainPageViewModel viewModel) : ICommand
    {
        private readonly MainPageViewModel _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            await _viewModel.ConvertFiles();
        }
    }
}
