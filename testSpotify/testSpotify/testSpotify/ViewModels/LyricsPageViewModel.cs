using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using testSpotify.Models;
using testSpotify.LocalModels;
using testSpotify.Services;
using Plugin.Toast;

namespace testSpotify.ViewModels
{
    class LyricsPageViewModel : BaseViewModel
    {
        private readonly MongoDBClass mongo = null;

        private ArtistModel artistcollection;

        private string artistName;
        private string albumName;
        private string trackName;
        private string lyrics;

        public string Lyrics
        {
            get { return lyrics; }
            set { lyrics = value; OnPropertyChanged(); }
        }
        public string TrackName
        {
            get { return trackName; }
            set { trackName = value; OnPropertyChanged(); }
        }
        public string AlbumName
        {
            get { return albumName; }
            set { albumName = value; OnPropertyChanged(); }
        }
        public string ArtistName
        {
            get { return artistName; }
            set { artistName = value; OnPropertyChanged(); }
        }

        public LyricsPageViewModel()
        {
            ArtistName = "Paolo Nutini";
            AlbumName = "Caustic Love";
            TrackName = "One Day";

            //Test Mongo DB
            /* mongo.InsertArtist(ArtistName, AlbumName, trackName);
            TrovaArtista(); */

            //Test DB Locale

            /*
            InserisciArtistaPreferito();
            TrovaArtistaPreferito();
            */

            App.Mongo.UpdateMongoDBArtist(ArtistName, AlbumName, TrackName);
        }
        


        private void InserisciArtistaPreferito()
        {
            App.Database.SaveArtistAsync(new LocalArtistModel()
            {
                ArtistName = this.ArtistName,
                AlbumName = this.AlbumName,
                TrackName = this.TrackName,
                Lyrics = null

            });
        }

        private async void TrovaArtistaPreferito()
        {

            try
            {
                List<LocalArtistModel> artistatrovato = await App.Database.GetArtistsAsync();

                if (artistatrovato != null)
                {
                    foreach (var temp in artistatrovato)
                    {
                        CrossToastPopUp.Current.ShowToastMessage(temp.ArtistName, Plugin.Toast.Abstractions.ToastLength.Long);
                    }
                }
                CancellaDBLocale(artistatrovato);
            }
            catch (Exception)
            {
                CrossToastPopUp.Current.ShowToastError("Errore durante la ricerca dell'artista preferito");
            }
        }
        private void CancellaDBLocale(List<LocalArtistModel> artistaTrovato)
        {
            try
            {
                foreach(var temp in artistaTrovato)
                {
                    App.Database.DeleteArtistAsync(temp);
                }
            }
            catch (Exception)
            {
                CrossToastPopUp.Current.ShowToastError("Errore durante la cancellazione dei preferiti");
            }
        }
    }
}
