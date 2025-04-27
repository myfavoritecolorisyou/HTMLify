

using HTMLify.Application.Services;
using HTMLify.Domain.Services;
using HTMLify.Presentation.ViewModels.Windows;
using HTMLify.Presentation.Views;
using HTMLify.Presentation.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace HTMLify
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // 서비스 등록
                services.AddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<AppSettingsService>();
                services.AddSingleton<FileProccessingService>();

                // MainWindow를 INavigationWindow로 등록
                services.AddSingleton<INavigationWindow, MainWindow>(); 
                services.AddSingleton<MainWindowViewModel>();

                // 페이지 및 ViewModel 등록
                _ = services.AddSingleton<MainPage>();
                _ = services.AddSingleton<MainPageViewModel>();
                _ = services.AddSingleton<PreviewPage>();
                _ = services.AddSingleton<PreviewPageViewModel>();
                _ = services.AddSingleton<SettingsPage>();
                _ = services.AddSingleton<SettingsPageViewModel>();
            })
            .Build();

        public static IServiceProvider Services => _host.Services;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = Services.GetRequiredService<INavigationWindow>();
            mainWindow.ShowWindow();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO: 예외처리
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MessageBox.Show($"Unhandled exception: {args.ExceptionObject}");
            };

            base.OnStartup(e);
        }

    }

}
