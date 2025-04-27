using Wpf.Ui.Abstractions.Controls;

namespace HTMLify.Presentation.Views.Pages
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : INavigableView<MainPageViewModel>
    {
        public MainPageViewModel ViewModel { get; }

        public MainPage(MainPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
