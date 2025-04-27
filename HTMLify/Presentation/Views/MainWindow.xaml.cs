using HTMLify.Presentation.ViewModels.Windows;
using HTMLify.Presentation.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace HTMLify.Presentation.Views
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();

            navigationService.SetNavigationControl(RootNavigation);
        }


        public void ShowWindow() => Show();
        public void CloseWindow() => Close();
        public INavigationView GetNavigation() => RootNavigation;
        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);
        public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
            RootNavigation.SetPageProviderService(navigationViewPageProvider);

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
