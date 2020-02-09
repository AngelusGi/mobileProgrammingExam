using MongoDB.Driver;
using System;
using System.Collections.Generic;
using testSpotify.LocalModels;
using Plugin.Toast;
using MusixMatch_API;
using SpotifyAPI.Web.Models;
using MusixMatch_API.APIMethods.Matcher;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using SpotifyAPI.Web.Enums;
using System.Threading.Tasks;

namespace testSpotify.ViewModels
{
    public class LyricsPageViewModel : BaseViewModel
    {
        private MusixMatchApi api;
        private PlaybackContext playback;

        private bool _isRefreshing = false;

        private string albumName;
        private string artistName;
        private string trackName;
        private string lyrics;
        private string albumImage;
        private string playerImage;

        private string _res;

        public ICommand AddCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand RewindCommand { get; set; }
        public ICommand ResumeCommand { get; set; }
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

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
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
        public string PlayerImage
        {
            get { return playerImage; }
            set { playerImage = value; OnPropertyChanged(); }
        }

        public LyricsPageViewModel()
        {
            AddCommand = new Command(() => AddToFavorites());
            ForwardCommand = new Command(() => Forward());
            RewindCommand = new Command(() => Rewind());
            ResumeCommand = new Command(() => Resume());
            api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
            PlayerImage = "ic_action_play.png";
        }

        #region PlayerPage
        private void Resume()
        {
            avaiability(async () =>
            {
                playback = await SpotifyApi.GetPlaybackAsync();

                if (playback.Context != null || playback.Item != null)
                {
                    if (playback.IsPlaying)
                    {
                        PlayerImage = "ic_action_play.png";
                        ErrorResponse x = await SpotifyApi.PausePlaybackAsync();
                    }
                    else
                    {
                        PlayerImage = "ic_action_pause.png";
                        ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(playback.Device.Id,string.Empty,
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
        #endregion


        #region LyricsPage

        private void AddToFavorites()
        {
            avaiability(() =>
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

            });
        }

        public void SetMatcherAsync()
        {
            avaiability(() => UpdateUI());
        }

        #endregion

        #region Common

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

        private async Task UpdateUI()
        {
            playback = await SpotifyApi.GetPlaybackAsync();
            AlbumName = playback.Item.Album.Name;
            AlbumImage = playback.Item.Album.Images.FirstOrDefault().Url;
            TrackName = playback.Item.Name;
            ArtistName = playback.Item.Artists.FirstOrDefault().Name;

            if (playback.IsPlaying)
            {
                PlayerImage = "ic_action_pause.png";
            }
            else
            {
                PlayerImage = "ic_action_play.png";
            }

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

                    if (playback.CurrentlyPlayingType.Equals(TrackType.Track))
                    {
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
                            CrossToastPopUp.Current.ShowToastMessage("login richiesto");
                        }
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastMessage("Impossibile scaricare il testo, non è una canzone");
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

        #endregion
    }
}