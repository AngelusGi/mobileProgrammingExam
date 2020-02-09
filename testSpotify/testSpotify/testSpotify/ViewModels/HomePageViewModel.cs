using Plugin.Toast;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using testSpotify.LocalModels;
using testSpotify.Views;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await UpdateUI();

                    IsRefreshing = false;
                });
            }
        }

        public ICommand SettingsCommand { get; set; }
        public ICommand RemoveCommand
        {
            get
            {
                return new Command<LocalArtistModel>(async artist =>
                {
                    var error = await App.Database.DeleteArtistAsync(artist);
                    if(error == null)
                    {
                        CrossToastPopUp.Current.ShowToastWarning("Database occupato, riprova più tardi");
                    }
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

        public async Task UpdateUI()
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
