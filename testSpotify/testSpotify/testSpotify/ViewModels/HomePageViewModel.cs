using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<LocalArtistModel> artists;

        public ICommand SettingsCommand { get; set; }
        public ICommand RemoveCommand
        {
            get
            {
                return new Command<LocalArtistModel>(async artist =>
                {
                    await App.Database.DeleteArtistAsync(artist);
                });
            }
        }

        public ObservableCollection<LocalArtistModel> Artists { get; set; }

        public HomePageViewModel()
        {
            Artists = new ObservableCollection<LocalArtistModel>();
            SettingsCommand = new Command(() => OpenSettings());
            UpdateUI();
        }

        private async void OpenSettings()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new Settings());
        }

        public async void UpdateUI()
        {
            List<LocalArtistModel> li = await App.Database.GetArtistsAsync();
            Artists.Clear();
            if (li.Count != 0)
            {
                li.ForEach(temp => Artists.Add(temp));
            }
        }

    }
}
