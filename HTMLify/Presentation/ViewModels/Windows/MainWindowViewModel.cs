using CommunityToolkit.Mvvm.ComponentModel;
using HTMLify.Presentation.ViewModels;
using HTMLify.Presentation.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace HTMLify.Presentation.ViewModels.Windows
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = [];

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = [];

        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
            {
                InitializeModel();
            }
        }


        private void InitializeModel()
        {
            ApplicationTitle = "HTMLify";

            NavigationItems =
            [
                new NavigationViewItem()
                {
                    Content = "Main",
                    Icon = new SymbolIcon{ Symbol = SymbolRegular.Home24},
                    TargetPageType = typeof(MainPage)
                },
                new NavigationViewItem()
                {
                    Content = "Preview",
                    Icon = new SymbolIcon{ Symbol = SymbolRegular.Laptop24},
                    TargetPageType = typeof(PreviewPage)
                }
            ];

            NavigationFooter =
            [
                new NavigationViewItem
                {
                    Content = "Settings",
                    Icon = new SymbolIcon{ Symbol = SymbolRegular.Settings24},
                    TargetPageType = typeof(SettingsPage)
                }
            ];

            _isInitialized = true;
        }
    }
}
