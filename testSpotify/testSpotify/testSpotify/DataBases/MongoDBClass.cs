using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text;
using testSpotify.Models;
using Plugin.Toast;

namespace testSpotify.Services
{
    class MongoDBClass
    {

        public IMongoDatabase Db { get; set; }
        public MongoClient Client { get; set; }



        public MongoDBClass(string database)
        {
            Client = new MongoClient(@"mongodb://10.64.194.4:27017");
            Db = Client.GetDatabase(database);
        }


        public void InsertRecord<T>(string Table, T Record)
        {
            var collection = Db.GetCollection<T>(Table);
            collection.InsertOne(Record);
        }
        //public List<T> LoadRecord<T>(string Table)
        //{
        //    var collection = Db.GetCollection<T>(Table);

        //    return collection.Find(new BsonDocument()).ToList();
        //}
        //public T LoadRecordById<T>(string Table, Guid Id)
        //{
        //    var collection = Db.GetCollection<T>(Table);
        //    var filter = Builders<T>.Filter.Eq("Id", Id);

        //    return collection.Find(filter).First();
        //}
        public T LoadRecordByName<T>(string Table, string Field, string Value)
        {
            var collection = Db.GetCollection<T>(Table);
            var filter = Builders<T>.Filter.Eq(Field, Value);
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

        public void InsertArtist(string artistName, string albumName, string trackName)
        {
            try
            {
                this.InsertRecord("Artist", new ArtistModel()
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
                                    TrackName = trackName
                                }
                            }
                        }
                    }
                });
            }
            catch (MongoException)
            {
                CrossToastPopUp.Current.ShowToastError("Errore durante l'inserimento dell'artista");
            }
        }
        public ArtistModel GetArtist(string ArtistName)
        {
            try
            {
                IMongoCollection<ArtistModel> collection = Db.GetCollection<ArtistModel>("Artist");
                try
                {
                    ArtistModel artist = this.LoadRecordByName<ArtistModel>("Artist", "ArtistName", ArtistName);
                    return artist;
                }
                catch (Exception)
                {
                    //CrossToastPopUp.Current.ShowToastError("Artista non trovato");
                    //Inserire Artista(?)
                }
            }
            catch (Exception)
            {
                CrossToastPopUp.Current.ShowToastError("Errore durante il caricamento degli artisti");
            }
            return null;
        }
    }

}
