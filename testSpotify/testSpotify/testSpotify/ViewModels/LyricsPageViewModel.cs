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
        public ICommand ForwardCommand { get; set; }
        public ICommand RewindCommand { get; set; }
        public ICommand ResumeCommand { get; set; }

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
            ForwardCommand = new Command(() => Forward());
            RewindCommand = new Command(() => Rewind());
            ResumeCommand = new Command(() => Resume());
            api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
        }

        private void Resume()
        {
            avaiability(async () =>
            {
                playback = await SpotifyApi.GetPlaybackAsync();

                if (playback.Context != null || playback.Item != null)
                {
                    if (playback.IsPlaying)
                    {
                        ErrorResponse x = await SpotifyApi.PausePlaybackAsync();
                    }
                    else
                    {
                        ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(playback.Device.Id, string.Empty,
                            new List<string>() { playback.Item.Uri }, "", playback.ProgressMs);
                    }
                }
            });



        }

        private void Rewind()
        {
            avaiability(async () =>
            {
                playback = await SpotifyApi.GetPlaybackAsync();

                if (playback.Context != null || playback.Item != null)
                {
                    ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(playback.Device.Id, string.Empty,
                        new List<string>() { playback.Item.Uri }, "", playback.ProgressMs - 10000);
                }
            });
        }

        private void Forward()
        {
            avaiability(async () =>
            {
                playback = await SpotifyApi.GetPlaybackAsync();

                if (playback.Context != null || playback.Item != null)
                {
                    ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(playback.Device.Id, string.Empty,
                        new List<string>() { playback.Item.Uri }, "", playback.ProgressMs + 10000);
                }
            });
        }

        private void AddToFavorites()
        {
            if (playback.Item != null)
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

        public void SetMatcherAsync()
        {
            avaiability(() => UpdateUI());
        }

        public void LoadLyrics(LocalArtistModel localArtistModel)
        {
            this.ArtistName = localArtistModel.ArtistName;
            this.AlbumName = localArtistModel.AlbumName;
            this.AlbumImage = localArtistModel.AlbumImage;
            this.trackName = localArtistModel.TrackName;
            this.Lyrics = localArtistModel.Lyrics;
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

        private async void avaiability(Action action)
        {
            if (SpotifyApi != null)
            {
                if ((await SpotifyApi.GetDevicesAsync()).Devices.Count != 0)
                {

                    playback = await SpotifyApi.GetPlaybackAsync();

                    if (Logged)
                    {

                        if (playback.Item != null || playback.Item.Name != (await SpotifyApi.GetPlaybackAsync()).Item.Name)
                        {
                            action();
                        }
                        else
                        {
                            CrossToastPopUp.Current.ShowToastMessage("Errore durante il download del testo");
                        }
                    }
                    else
                    {
                        //aggiornare la pagina dal database locale
                        CrossToastPopUp.Current.ShowToastMessage("login richiesto");
                    }
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastMessage("Accedi a spotify da uno dei tuoi dispositivi!");
                }

            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("Login Richiesto");
            }

        }
    }
}