using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace HTMLify.Presentation.ViewModels
{
    public partial class BaseViewModel : ObservableObject, INavigationAware
    {
        public Task OnNavigatedFromAsync()
        {
            OnNavigatingFrom();
            return Task.CompletedTask;
        }
        public virtual void OnNavigatingFrom() { }

        public virtual Task OnNavigatedToAsync()
        {
            OnNavigatingTo();
            return Task.CompletedTask;
        }
        public virtual void OnNavigatingTo() { }
    }
}
