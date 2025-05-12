using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;
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

            Loaded += PreviewPage_Loaded;
        }

        private async void PreviewPage_Loaded(object sender, RoutedEventArgs e)
        {
            // WebView2 캐시 폴더를 AppData 쪽으로 이동
            string userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HTMLify", "WebView2");

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await WebView.EnsureCoreWebView2Async(env);
        }

    }
}
