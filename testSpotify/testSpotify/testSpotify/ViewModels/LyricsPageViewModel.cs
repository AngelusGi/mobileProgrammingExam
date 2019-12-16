using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using testSpotify.Models;
using testSpotify.Services;

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
            //InserisciArtista();
            TrovaArtista();
        }

        private void InserisciArtista()
        {
            try
            {
                mongo.InsertRecord("Artist", new ArtistModel()
                {
                    ArtistName = this.ArtistName,
                    Albums = new List<AlbumModel>()
                    {
                        new AlbumModel()
                        {
                            AlbumName = this.AlbumName,
                            Tracks = new List<TrackModel>()
                            {
                                new TrackModel()
                                {
                                    TrackName = this.TrackName
                                }
                            }
                        }
                    }
                });
            }
            catch (MongoException e)
            {
                //Gestire l'errore
            }
        }

        private string TrovaArtista()
        {
            IMongoCollection<ArtistModel> collection = mongo.Db.GetCollection<ArtistModel>("Artist"); 
            ArtistModel artistcollection = null;

            try
            {
                artistcollection = mongo.LoadRecordByName<ArtistModel>("Artist", "ArtistName", ArtistName);
                
                //Se l'artista è presente
                if (artistcollection != null)
                {
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
            catch (InvalidOperationException e)
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
            
            //Se l'artista è presente ma l'album no, costruzione a manella del json per l'update dell'album
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

            //Ricercad dell'album
            while (i < artistcollection.Albums.Count)
            {
                //Se l'album è stato trovato, mi salvo la sua posizione
                if (artistcollection.Albums[i].AlbumName == this.AlbumName)
                {
                    //Album Trovato
                    AlbumPos = i;
                    break;
                }
                i++;
            }
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

            while (i < artistcollection.Albums[albumPos].Tracks.Count)
            {
                if (artistcollection.Albums[albumPos].Tracks[i].TrackName == this.TrackName)
                {
                    //Traccia Trovata
                    TrackPos = i;
                    break;
                }
                i++;
            }
            //Se la traccia non è stata trovata
            if (TrackPos == -1)
            {
                artistcollection.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = this.TrackName, Lyrics = null });

                ReplaceOneResult ReplaceResult = collection.ReplaceOne(ArtistFilter, artistcollection, new UpdateOptions { IsUpsert = true });
                //Traccia aggiunta
            }
            else
            {
                if (artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics == null)
                {
                    //Traccia Non presente
                }
                else
                {
                    return artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics;
                }
            }
            return null;
        }

    }
}
