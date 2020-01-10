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
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace testSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private Token lastToken;
        SpotifyWebAPI _api = null;
        private Token _token = null;


        private HttpClient httpClient = new HttpClient();
        private HttpRequestMessage request = null;
        private string res;

        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command(() => TestAuth());
        }

        public ICommand TestAuthCommand { get; }

        private async void TestAuth()
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

            PlaybackContext context = await SpotifyApi.GetPlaybackAsync();
            

            MusixMatch_API.APIMethods.Matcher.MatcherLyricsGet matcher = new MusixMatch_API.APIMethods.Matcher.MatcherLyricsGet() { SongArtist = context.Item.Artists.FirstOrDefault().Name, SongTitle = context.Item.Name };

            api.MatcherLyricsGet(matcher, result =>
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