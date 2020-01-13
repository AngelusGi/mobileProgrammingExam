﻿using MongoDB.Driver;
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

namespace testSpotify.ViewModels
{
    internal class LyricsPageViewModel : BaseViewModel
    {
        private MusixMatchApi api;
        private string albumName;
        private string artistName;
        private string trackName;
        private string lyrics;
        private string albumImage;
        private string _res;

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
            api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
        }
        public async void SetMatcherAsync()
        {

            PlaybackContext context = await SpotifyApi.GetPlaybackAsync();
            if (context != null)
            {
                AlbumName = "RIPRODUZIONE DA ALBUM\n" + context.Item.Album.Name;
                AlbumImage = context.Item.Album.Images.FirstOrDefault().Url;
                TrackName = context.Item.Name;
                ArtistName = context.Item.Artists.FirstOrDefault().Name;

                try
                {
                    MatcherLyricsGet matcher = new MatcherLyricsGet()
                    {
                        SongArtist = context.Item.Artists.FirstOrDefault().Name,
                        SongTitle = context.Item.Name
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
        }
    }
}