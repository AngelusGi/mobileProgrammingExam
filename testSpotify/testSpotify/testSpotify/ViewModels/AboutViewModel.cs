using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Plugin.Toast;
using MusixMatch_API;
using MusixMatch_API.APIMethods.Artist;

namespace testSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        string res = string.Empty;
        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command(() => TestAuth());
        }

        public ICommand TestAuthCommand { get; }

        private void TestAuth()
        {

            var api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
            var artistSearch = new ArtistSearch { ArtistName = "Paolo Nutini" };
            //var lyricsSearch = new MusixMatch_API.APIMethods.Track.TrackSearch { FilterOnArtistId = artistSearch.MusicBrainzArtistId };
            api.ArtistSearch(artistSearch, result =>
            {
                // Your results in result
                res = result.
            }, error =>
            {
                // Something went wrong. Error is in error string.
            });

            CrossToastPopUp.Current.ShowToastMessage(res);

        }
    }
}