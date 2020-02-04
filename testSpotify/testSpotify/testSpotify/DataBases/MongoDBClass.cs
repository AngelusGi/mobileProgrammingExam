using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using testSpotify.Models;
using Plugin.Toast;
using MusixMatch_API;
using MusixMatch_API.APIMethods.Matcher;
using System.Linq;
using System.Diagnostics;

namespace testSpotify.Services
{
    public class MongoDbClass
    {
        private static MusixMatchApi api;
        private string tmpLyrics = null;

        public IMongoDatabase Db { get; set; }
        public MongoClient Client { get; set; }

        /// <summary>
        /// Instanzia la connessione con il DB Mongo
        /// </summary>
        /// <param name="database"></param>
        /// <param name="mongodbpath"></param>
        public MongoDbClass(string database, string mongodbpath)
        {
            api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
            Client = new MongoClient(mongodbpath);
            Db = Client.GetDatabase(database);
        }

        /// <summary>
        /// Insrisce un record nel DB Mongo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="record"></param>
        public void InsertRecord<T>(string table, T record)
        {
            var collection = Db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        /// <summary>
        /// Ottiene il record invocato
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> LoadRecord<T>(string Table)
        {
            var collection = Db.GetCollection<T>(Table);

            return collection.Find(new BsonDocument()).ToList();
        }
        public T LoadRecordById<T>(string Table, Guid ID)
        {
            var collection = Db.GetCollection<T>(Table);
            var filter = Builders<T>.Filter.Eq("ID", ID);

            return collection.Find(filter).First();
        }
        public T LoadRecordByName<T>(string table, string field, string value)
        {
            var collection = Db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            try
            {
                var result = collection.Find(filter).FirstOrDefault();
                return result;
            }
            catch (System.InvalidOperationException)
            {
                return default;
            }
        }

        /// <summary>
        /// Metodo che mantiene sempre aggiornato il database
        /// </summary>
        /// <param name="artistName">Chiave di ricerca</param>
        /// <returns>Se presente, ritorna il testo della canzone</returns>
        public string UpdateMongoDbArtist(string artistName, string albumName, string trackName, string lyrics = null)
        {
            IMongoCollection<ArtistModel> collection = Db.GetCollection<ArtistModel>("Artist");
            ArtistModel artistcollection = null;

            try
            {
                artistcollection = LoadRecordByName<ArtistModel>("Artist", "ArtistName", artistName);

                if (artistcollection != null)
                {
                    lyrics = TrovaAlbum(artistcollection, collection, albumName, trackName, lyrics);
                    if (lyrics != null)
                    {
                        return lyrics;
                    }
                }
                else
                {
                    InserisciArtista(artistName, albumName, trackName, lyrics);
                }
            }
            catch (InvalidOperationException e)
            {
                CrossToastPopUp.Current.ShowToastMessage("Error : " + e.Message);
            }
            return null;
        }

        private string TrovaAlbum(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection, string albumName, string trackName, string lyrics = null)
        {
            FilterDefinition<ArtistModel> ArtistFilter = Builders<ArtistModel>.Filter.Eq("ArtistName", artistcollection.ArtistName);


            UpdateDefinition<ArtistModel> UpdateAlbum = Builders<ArtistModel>.Update.Push("Albums",
                new AlbumModel
                {
                    AlbumName = albumName,
                    Tracks = new List<TrackModel>
                    {
                        new TrackModel
                        {
                            TrackName = trackName,
                            Lyrics = lyrics
                        }
                    }
                });
            int AlbumPos = -1;
            int i = 0;

            while (i < artistcollection.Albums.Count)
            {
                if (artistcollection.Albums[i].AlbumName == albumName)
                {
                    AlbumPos = i;
                    break;
                }
                i++;
            }
            if (AlbumPos == -1)
            {
                UpdateResult updateResult = collection.UpdateOne(ArtistFilter, UpdateAlbum, new UpdateOptions { IsUpsert = true });
            }
            else
            {
                lyrics = TrovaTraccia(artistcollection, collection, ArtistFilter, AlbumPos, trackName, UpdateAlbum, lyrics);
                if (lyrics != null)
                {
                    return lyrics;
                }
            }
            return null;
        }

        private string TrovaTraccia(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection, FilterDefinition<ArtistModel> artistFilter,
                int albumPos, string trackName, UpdateDefinition<ArtistModel> updateAlbum, string lyrics = null)
        {
            int TrackPos = -1;
            int i = 0;

            while (i < artistcollection.Albums[albumPos].Tracks.Count)
            {
                if (artistcollection.Albums[albumPos].Tracks[i].TrackName == trackName)
                {
                    TrackPos = i;
                    break;
                }
                i++;
            }
            if (TrackPos == -1)
            {
                artistcollection.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = trackName, Lyrics = lyrics });

                ReplaceOneResult ReplaceResult = collection.ReplaceOne(artistFilter, artistcollection, new ReplaceOptions { IsUpsert = true });
            }
            else
            {
                if (artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics == null)
                {
                    SetLyrics(artistcollection.ArtistName, artistcollection.Albums[albumPos].Tracks[TrackPos].TrackName);

                    artistcollection.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = trackName, Lyrics = lyrics });
                    ReplaceOneResult updateres = collection.ReplaceOne(artistFilter, artistcollection, new ReplaceOptions { IsUpsert = true });

                    return lyrics;
                }
                else
                {
                    return artistcollection.Albums[albumPos].Tracks[TrackPos].Lyrics;
                }
            }
            return lyrics;
        }

        private void InserisciArtista(string artistName, string albumName, string TrackName, string lyrics = null)
        {
            try
            {
                InsertRecord("Artist", new ArtistModel()
                {
                    ArtistName = artistName,
                    Albums = new List<AlbumModel>()
                    {
                        new AlbumModel()
                        {
                            AlbumName = albumName,
                            Tracks = new List<TrackModel>()
                            {
                                new TrackModel()
                                {
                                    TrackName = TrackName,
                                    Lyrics = lyrics
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
                CrossToastPopUp.Current.ShowToastMessage("Errore durante l'update");
            }
        }

        public void SetLyrics(string artistName, string trackName)
        {
            var api = new MusixMatchApi("5f18a4bfea8c334574b0860a8b638409");
            string _res = null;

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
                        tmpLyrics = result.LyricsBody;
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