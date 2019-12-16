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
                //MessageBox.Show("Artista Inserito");
                //Achtung();
            }
            catch (MongoException e)
            {
               // _ = MessageBox.Show("Errore : " + e.Message);
            }
        }
        private string TrovaArtista()
        {
            IMongoCollection<ArtistModel> collection = mongo.Db.GetCollection<ArtistModel>("Artist"); 
            ArtistModel artistcollection = null;

            try
            {
                artistcollection = mongo.LoadRecordByName<ArtistModel>("Artist", "ArtistName", ArtistName);
                
                if (artistcollection != null)
                {
                    string lyrics = TrovaAlbum(artistcollection, collection);
                    if (lyrics != null)
                    {
                        return lyrics;
                    }
                }
                else
                {
                    //InserisciArtista();
                    //Achtung();
                }
            }
            catch (InvalidOperationException e)
            {
                //InserisciArtista();
                //Gestire Errore
            }
            return null;
        }

        [Obsolete]
        private string TrovaAlbum(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection)
        {
            FilterDefinition<ArtistModel> ArtistFilter = Builders<ArtistModel>.Filter.Eq("ArtistName", this.ArtistName);
            

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

            while (i < artistcollection.Albums.Count)
            {
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
                UpdateResult updateResult = collection.UpdateOne(ArtistFilter, UpdateAlbum, new UpdateOptions { IsUpsert = true });
                //Album Inserito
            }
            else
            {
                string lyrics = TrovaTraccia(artistcollection, collection, ArtistFilter, AlbumPos);
                if (lyrics != null)
                {
                    return lyrics;
                }
            }
            return null;
        }

        [Obsolete]
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
