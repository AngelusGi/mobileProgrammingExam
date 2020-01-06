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
using MusixMatch_API.APIMethods.Track;
using MusixMatch_API.ReturnTypes;
using MusixMatch_API.APIMethods.Artist;

namespace testSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        string res = string.Empty;
        int artistId;
        string trackId;
        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command(() => TestAuth());
        }

        public ICommand TestAuthCommand { get; }

        private void TestAuth()
        {

            var api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
            //var artistSearch = new ArtistSearch { ArtistName = "Paolo Nutini" };
            //api.ArtistSearch(artistSearch, result =>
            //{
            //    // Your results in result

            //    artistId = result.FirstOrDefault().Artist.ArtistId;
            //}, error =>
            //{
            //    // Something went wrong. Error is in error string.
            //});

            //var lyricsSearch = new TrackSearch() { FilterOnArtistId = artistId };
            //api.TrackSearch(lyricsSearch, result =>
            //{
            //    trackId = result.FirstOrDefault().Track.TrackName;
            //}, error =>
            //{

            //});

            MusixMatch_API.APIMethods.Matcher.MatcherLyricsGet temp = new MusixMatch_API.APIMethods.Matcher.MatcherLyricsGet() { SongArtist = "Paolo Nutini", SongTitle = "Iron Sky" };

            api.MatcherLyricsGet(temp, result =>
            {
                res = result.LyricsBody;
            }, error =>
            {
                res = error.FirstOrDefault().ToString();
            });

            CrossToastPopUp.Current.ShowToastMessage(res);

        }
    }
}