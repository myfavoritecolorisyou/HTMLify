using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Wpf.Ui.Abstractions;

namespace HTMLify.Domain.Services
{
    public class NavigationViewPageProvider : INavigationViewPageProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationViewPageProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // GetPage 메서드 수정: DI를 통해 페이지 인스턴스를 반환
        public object? GetPage(Type pageType)
        {
            if (pageType == typeof(Presentation.Views.Pages.MainPage))
                return _serviceProvider.GetRequiredService<Presentation.Views.Pages.MainPage>();

            if (pageType == typeof(Presentation.Views.Pages.PreviewPage))
                return _serviceProvider.GetRequiredService<Presentation.Views.Pages.PreviewPage>();

            if (pageType == typeof(Presentation.Views.Pages.SettingsPage))
                return _serviceProvider.GetRequiredService<Presentation.Views.Pages.SettingsPage>();

            // 알 수 없는 페이지 타입 처리
            throw new ArgumentException($"Unknown page type: {pageType.FullName}");
        }
    }
}