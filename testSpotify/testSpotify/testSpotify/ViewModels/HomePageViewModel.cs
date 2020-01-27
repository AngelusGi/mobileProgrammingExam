using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using testSpotify.LocalModels;
using testSpotify.Models;
using testSpotify.Views;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private List<LocalArtistModel> artists;

        public ICommand SettingsCommand{ get; set; }

        public List<LocalArtistModel> Artists
        {
            get { return artists; }
            set
            {
                artists = value; OnPropertyChanged();
            }
        }

        public HomePageViewModel()
        {
            SettingsCommand = new Command(() => OpenSettings());
            UpdateUI();
        }

        private async void OpenSettings()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new Settings());
        }

        public async void UpdateUI()
        {
            Artists = await App.Database.GetArtistsAsync();
        }
        
    }
}
