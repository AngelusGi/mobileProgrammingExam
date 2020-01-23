using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using testSpotify.Views;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; set; }
        public ICommand ContinueCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(() => LoginCall());
            ContinueCommand = new Command(() => ContinueCall());
        }

        private async void ContinueCall()
        {
            Logged = false;
            App.Current.MainPage = new NavigationPage(new MainPage());
        }

        private async void LoginCall()
        {
            Logged = true;
            App.Current.MainPage = new NavigationPage(new AuthView());
        }
    }
}
