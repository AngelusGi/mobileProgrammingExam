using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using testSpotify.Views;
using testSpotify.Views.Spotify;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string loginButtonText;
        private bool restaCollegato;

        public ICommand LoginButtonCommand { get; set; }

        public bool RestaCollegato
        {
            get { return restaCollegato; }
            set
            {
                restaCollegato = value; 
                OnPropertyChanged();
                Preferences.Set("AutoLogin", restaCollegato);
            }
        }


        public string LoginButtonText
        {
            get { return loginButtonText; }
            set
            {
                loginButtonText = value; OnPropertyChanged();
            }
        }
        public SettingsViewModel()
        {
            RestaCollegato = Preferences.Get("AutoLogin", false);

            if (Logged)
                LoginButtonText = "Log-out";
            else
                LoginButtonText = "Log-in";

            LoginButtonCommand = new Command(() => LoginButton());
        }

        private async void LoginButton()
        {
            if (Logged)
            {
                Logged = false;
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
            else
            {
                Logged = true;
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new AuthView()));
            }
        }
    }
}
