using System.Windows.Controls;

namespace HTMLify.Presentation.Views.Pages
{
    /// <summary>
    /// PreviewPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreviewPage : Page
    {
        public PreviewPageViewModel ViewModel { get; }

        public PreviewPage(PreviewPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
