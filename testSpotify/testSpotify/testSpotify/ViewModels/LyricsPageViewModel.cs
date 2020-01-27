using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using testSpotify.Models;
using testSpotify.LocalModels;
using testSpotify.Services;
using Plugin.Toast;
using MusixMatch_API;
using SpotifyAPI.Web.Models;
using MusixMatch_API.APIMethods.Matcher;
using System.Linq;
using System.Diagnostics;
using SpotifyAPI.Web;
using System.Windows.Input;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    public class LyricsPageViewModel : BaseViewModel
    {
        private MusixMatchApi api;
        private PlaybackContext playback;

        private string albumName;
        private string artistName;
        private string trackName;
        private string lyrics;
        private string albumImage;
        private string _res;

        public ICommand AddCommand { get; set; }

        public string AlbumImage
        {
            get { return albumImage; }
            set
            {
                albumImage = value; OnPropertyChanged();
            }
        }
        public string Lyrics
        {
            get { return lyrics; }
            set
            {
                lyrics = value; OnPropertyChanged();
            }
        }
        public string TrackName
        {
            get { return trackName; }
            set
            {
                trackName = value; OnPropertyChanged();
            }
        }
        public string ArtistName
        {
            get { return artistName; }
            set
            {
                artistName = value; OnPropertyChanged();
            }
        }
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                albumName = value;
                OnPropertyChanged();
            }
        }

        public LyricsPageViewModel()
        {
            AddCommand = new Command(() => AddToFavorites());
            api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
        }

        private void AddToFavorites()
        {
            if(playback.Item != null)
            {
                App.Database.SaveArtistAsync(new LocalArtistModel()
                {
                    ArtistName = this.ArtistName,
                    AlbumName = this.AlbumName,
                    TrackName = this.trackName,
                    AlbumImage = this.AlbumImage,
                    Lyrics = this.Lyrics
                });
                CrossToastPopUp.Current.ShowToastMessage("Aggiunto ai preferiti!");
            }
            else
            {
                //Non lo puoi fare se non stai ascoltando nulla oppure se il testo lo hai preso dal tuo database
            }
        }

        public async void SetMatcherAsync()
        {
            if (SpotifyApi != null)
            {
                playback = await SpotifyApi.GetPlaybackAsync();

                if (Logged)
                {

                    if (playback.Item != null || playback.Item.Name != (await SpotifyApi.GetPlaybackAsync()).Item.Name)
                    {
                        UpdateUI();
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastMessage("Errore durante il download del testo");
                    }
                }
                else
                {
                    //aggiornare la pagina dal database locale
                    CrossToastPopUp.Current.ShowToastMessage("login richiesto per download automatico del testo");
                }
            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("login richiesto per download automatico del testo");
            }
        }



        private void SetLyrics(string artistName, string albumName, string trackName)
        {
            try
            {
                MatcherLyricsGet matcher = new MatcherLyricsGet()
                {
                    SongArtist = artistName,
                    SongTitle = trackName
                };

                api.MatcherLyricsGet(
                    matcher,
                    result =>
                    {
                        Lyrics = result.LyricsBody;
                    },
                    error => { _res = error.FirstOrDefault().ToString(); });
            }
            catch (NullReferenceException nullReference)
            {
                Debug.AutoFlush = true;
                Debug.Print($"NullReferenceException:" +
                            $"\n\tMessage: {nullReference.Message}" +
                            $"\n\tSource: {nullReference.Source}" +
                            $"\n\tStack: {nullReference.StackTrace}");
            }
        }

        private async void UpdateUI()
        {
            playback = await SpotifyApi.GetPlaybackAsync();
            AlbumName = playback.Item.Album.Name;
            AlbumImage = playback.Item.Album.Images.FirstOrDefault().Url;
            TrackName = playback.Item.Name;
            ArtistName = playback.Item.Artists.FirstOrDefault().Name;


            SetLyrics(playback.Item.Artists.FirstOrDefault().Name, playback.Item.Album.Name, playback.Item.Name);

            App.Mongo.UpdateMongoDbArtist(playback.Item.Artists.FirstOrDefault().Name, playback.Item.Album.Name, playback.Item.Name, Lyrics);
        }

        public void LoadLyrics(LocalArtistModel localArtistModel)
        {
            this.ArtistName = localArtistModel.ArtistName;
            this.AlbumName = localArtistModel.AlbumName;
            this.AlbumImage = localArtistModel.AlbumImage;
            this.trackName = localArtistModel.TrackName;
            this.Lyrics = localArtistModel.Lyrics;
        }
    }
}