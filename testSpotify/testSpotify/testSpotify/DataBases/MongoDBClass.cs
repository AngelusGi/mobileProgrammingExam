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
    public class MongoDbClass
    {
        public IMongoDatabase Db { get; set; }
        public MongoClient Client { get; set; }

        /// <summary>
        /// Instanzia la connessione con il DB Mongo
        /// </summary>
        /// <param name="database"></param>
        /// <param name="mongodbpath"></param>
        public MongoDbClass(string database, string mongodbpath)
        {
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
        public string UpdateMongoDbArtist(string artistName, string albumName, string trackName)
        {
            IMongoCollection<ArtistModel> collection = Db.GetCollection<ArtistModel>("Artist");

            try
            {
                //cerco l'artista nel database di mongo
                var artistfound = LoadRecordByName<ArtistModel>("Artist", "ArtistName", artistName);

                //Se l'artista è presente
                if (artistfound != null)
                {
                    //cerco nell'album
                    string lyrics = UpdateAlbum(artistfound, collection, albumName, trackName);
                    if (lyrics != null)
                    {
                        return lyrics;
                    }
                }
                //Se l'artista non è presente
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


        private string UpdateAlbum(ArtistModel artistFound, IMongoCollection<ArtistModel> collection, string albumName,
            string trackName)
        {
            //Costruzione a manella del filtro di ricerca del Nome dell'artista
            FilterDefinition<ArtistModel> artistFilter =
                Builders<ArtistModel>.Filter.Eq("ArtistName", artistFound.ArtistName);

            //Se l'artista fosse presente ma l'album no, costruzione a manella del json per l'update dell'album
            var updateAlbum = Builders<ArtistModel>.Update.Push("Albums", new AlbumModel
            {
                AlbumName = albumName,
                Tracks = new List<TrackModel>
                {
                    new TrackModel
                    {
                        TrackName = trackName,
                        Lyrics = null
                    }
                }
            });
            
            int albumPos = -1;
            int index = 0;

            //Ricerca dell'album
            while (index < artistFound.Albums.Count)
            {
                //Se l'album è stato trovato, mi salvo la sua posizione tra i vari album dell'artista
                if (artistFound.Albums[index].AlbumName == albumName)
                {
                    //Album Trovato
                    CrossToastPopUp.Current.ShowToastMessage(artistFound.Albums[index].AlbumName,
                        Plugin.Toast.Abstractions.ToastLength.Long);
                    albumPos = index;
                    break;
                }

                index++;
            } //se l'album non è stato trovato, aggiungo alla lista degli album della lista quello che stavo ricercando

            if (albumPos == -1)
            {
                //Faccio la richiesta di update dell'artista con l'album aggiornato
                _ = collection.UpdateOne(artistFilter, updateAlbum, new UpdateOptions {IsUpsert = true});
                //Album Inserito
            }
            else
            {
                //Se l'album è stato trovato, vado a cercarmi la traccia
                string lyrics = UpdateTrack(artistFound, collection, artistFilter, albumPos, trackName);
                //Se ho trovato il testo della canzone, lo restituisco
                if (lyrics != null)
                {
                    return lyrics;
                }
            }

            return null;
        }

        private string UpdateTrack(ArtistModel artistFound, IMongoCollection<ArtistModel> collection,
            FilterDefinition<ArtistModel> artistFilter, int albumPos, string trackName)
        {
            int trackPos = -1;
            int index = 0;

            var updateAlbum = Builders<ArtistModel>.Update.Push("Albums",
                new AlbumModel
                {
                    AlbumName = artistFound.Albums[albumPos].AlbumName,
                    Tracks = new List<TrackModel>
                    {
                        new TrackModel
                        {
                            TrackName = trackName,
                            Lyrics = null
                        }
                    }
                });


            //cerco tra le tracce dell'album se la canzone che sto cercando è presente
            while (index < artistFound.Albums[albumPos].Tracks.Count)
            {
                if (artistFound.Albums[albumPos].Tracks[index].TrackName == trackName)
                {
                    //Traccia Trovata, mi salvo la sua posizione 
                    trackPos = index;
                    break;
                }

                index++;
            }

            //Se la traccia non è stata trovata
            if (trackPos == -1)
            {
                //OLD
                //artistFound.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = trackName, Lyrics = null });
                //_ = collection.ReplaceOne(ArtistFilter, artistFound, new ReplaceOptions { IsUpsert = true });


                //NEW
                //Aggiorno l'album con la nuova traccia
                _ = collection.UpdateOne(artistFilter, updateAlbum, new UpdateOptions {IsUpsert = true});
                //Traccia aggiunta
            }
            else
            {
                if (artistFound.Albums[albumPos].Tracks[trackPos].Lyrics == null)
                {
                    //Se la traccia è stata trovata ma non ho il testo della canzone, dovrò in qualche modo aggiungere il testo
                }
                else
                {
                    //Se il testo è presente, lo ritorno 
                    return artistFound.Albums[albumPos].Tracks[trackPos].Lyrics;
                }
            }

            return null;
        }
    }
}