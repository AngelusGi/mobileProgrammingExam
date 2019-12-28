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
    public class MongoDBClass
    {

        public IMongoDatabase Db { get; set; }
        public MongoClient Client { get; set; }



        public MongoDBClass(string database, string mongodbpath)
        {
            Client = new MongoClient(mongodbpath);
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

        /// <summary>
        /// Metodo che mantiene sempre aggiornato il database
        /// </summary>
        /// <param name="artistName">Chiave di ricerca</param>
        /// <returns>Se presente, ritorna il testo della canzone</returns>
        public string UpdateMongoDBArtist(string artistName, string albumName, string trackName)
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
        private string UpdateAlbum(ArtistModel artistFound, IMongoCollection<ArtistModel> collection, string albumName, string trackName)
        {
            //Costruzione a manella del filtro di ricerca del Nome dell'artista
            FilterDefinition<ArtistModel> ArtistFilter = Builders<ArtistModel>.Filter.Eq("ArtistName", artistFound.ArtistName);

            //Se l'artista fosse presente ma l'album no, costruzione a manella del json per l'update dell'album
            var UpdateAlbum = Builders<ArtistModel>.Update.Push("Albums",
                new AlbumModel
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
            int AlbumPos = -1;
            int i = 0;

            //Ricerca dell'album
            while (i < artistFound.Albums.Count)
            {
                //Se l'album è stato trovato, mi salvo la sua posizione tra i vari album dell'artista
                if (artistFound.Albums[i].AlbumName == albumName)
                {
                    //Album Trovato
                    CrossToastPopUp.Current.ShowToastMessage(artistFound.Albums[i].AlbumName, Plugin.Toast.Abstractions.ToastLength.Long);
                    AlbumPos = i;
                    break;
                }
                i++;
            }//se l'album non è stato trovato, aggiungo alla lista degli album della lista quello che stavo ricercando
            if (AlbumPos == -1)
            {
                //Faccio la richiesta di update dell'artista con l'album aggiornato
                _ = collection.UpdateOne(ArtistFilter, UpdateAlbum, new UpdateOptions { IsUpsert = true });
                //Album Inserito
            }
            else
            {
                //Se l'album è stato trovato, vado a cercarmi la traccia
                string lyrics = UpdateTrack(artistFound, collection, ArtistFilter, AlbumPos, trackName);
                //Se ho trovato il testo della canzone, lo restituisco
                if (lyrics != null)
                {
                    return lyrics;
                }
            }
            return null;
        }
        private string UpdateTrack(ArtistModel artistcollection, IMongoCollection<ArtistModel> collection, FilterDefinition<ArtistModel> ArtistFilter, int albumPos,string trackName)
        {
            int TrackPos = -1;
            int i = 0;

            //cerco tra le tracce dell'album se la canzone che sto cercando è presente
            while (i < artistcollection.Albums[albumPos].Tracks.Count)
            {
                if (artistcollection.Albums[albumPos].Tracks[i].TrackName == trackName)
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
                artistcollection.Albums[albumPos].Tracks.Add(new TrackModel { TrackName = trackName, Lyrics = null });
                //Ed aggiorno il json
                /*ReplaceOneResult ReplaceResult*/
                _ = collection.ReplaceOne(ArtistFilter, artistcollection, new ReplaceOptions { IsUpsert = true });
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
    }

}
