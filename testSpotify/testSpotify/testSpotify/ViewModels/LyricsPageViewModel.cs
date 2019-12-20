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
            mongo = new MongoDBClass("LyricsfyTest");
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
        }
        private string TrovaArtista()
        {
            IMongoCollection<ArtistModel> collection = mongo.Db.GetCollection<ArtistModel>("Artist"); 
            ArtistModel artistcollection = null;

            try
            {
                //cerco l'artista nel database di mongo
                artistcollection = mongo.LoadRecordByName<ArtistModel>("Artist", "ArtistName", ArtistName);
                
                //Se l'artista è presente
                if (artistcollection != null)
                {
                    //cerco nell'album
                    string lyrics = TrovaAlbum(artistcollection, collection);
                    if (lyrics != null)
                    {
                        return lyrics;
                    }
                } //Se l'artista non è presente
                else
                {
                    //InserisciArtista();
                }
            } 
            catch (InvalidOperationException)
            {
                //InserisciArtista();
                //Gestire Errore
            }
            return null;
        }
        private string TrovaAlbum(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection)
        {
            //Costruzione a manella del filtro di ricerca del Nome dell'artista
            FilterDefinition<ArtistModel> ArtistFilter = Builders<ArtistModel>.Filter.Eq("ArtistName", this.ArtistName);
            
            //Se l'artista fosse presente ma l'album no, costruzione a manella del json per l'update dell'album
            var UpdateAlbum = Builders<ArtistModel>.Update.Push("Albums",
                new AlbumModel
                {
                    AlbumName = this.AlbumName,
                    Tracks = new List<TrackModel>
                    {
                        new TrackModel
                        {
                            TrackName = this.TrackName,
                            Lyrics = null
                        }
                    }
                });
            int AlbumPos = -1;
            int i = 0;

            //Ricerca dell'album
            while (i < artistcollection.Albums.Count)
            {
                //Se l'album è stato trovato, mi salvo la sua posizione tra i vari album dell'artista
                if (artistcollection.Albums[i].AlbumName == this.AlbumName)
                {
                    //Album Trovato
                    CrossToastPopUp.Current.ShowToastMessage(artistcollection.Albums[i].AlbumName, Plugin.Toast.Abstractions.ToastLength.Long);
                    AlbumPos = i;
                    break;
                }
                i++;
            }//se l'album non è stato trovato, aggiungo alla lista degli album della lista quello che stavo ricercando
            if (AlbumPos == -1)
            {
                //Faccio la richiesta di update dell'artista con l'album aggiornato
                UpdateResult updateResult = collection.UpdateOne(ArtistFilter, UpdateAlbum, new UpdateOptions { IsUpsert = true });
                //Album Inserito
            }
            else
            {
                //Se l'album è stato trovato, vado a cercarmi la traccia
                string lyrics = TrovaTraccia(artistcollection, collection, ArtistFilter, AlbumPos);
                //Se ho trovato il testo della canzone, lo restituisco
                if (lyrics != null)
                {
                    return lyrics;
                }
            }
            return null;
        }        
        private string TrovaTraccia(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection, FilterDefinition<ArtistModel> ArtistFilter, int albumPos)
        {
            int TrackPos = -1;
            int i = 0;

            //cerco tra le tracce dell'album se la canzone che sto cercando è presente
            while (i < artistcollection.Albums[albumPos].Tracks.Count)
            {
                if (artistcollection.Albums[albumPos].Tracks[i].TrackName == this.TrackName)
                {
                    //Traccia Trovata, mi salvo la sua posizione 
                    TrackPos = i;
                    break;
                }
                i++;
            }
            //Se la traccia non è stata trovata
            if (TrackPos == -1)
            {
                //Aggiungo la nuova traccia tra quelle dell'album di questo artista
                artistcollection.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = this.TrackName, Lyrics = null });
                //Ed aggiorno il json
                ReplaceOneResult ReplaceResult = collection.ReplaceOne(ArtistFilter, artistcollection, new UpdateOptions { IsUpsert = true });
                //Traccia aggiunta
            }
            else
            {
                if (artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics == null)
                {
                    //Se la traccia è stata trovata ma non ho il testo della canzone, dovrò in qualche modo aggiungere il testo
                }
                else
                {
                    //Se il testo è presente, lo ritorno 
                    return artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics;
                }
            }
            return null;
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
