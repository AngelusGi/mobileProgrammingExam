using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text;

namespace testSpotify.Services
{
    class MongoDBClass
    {

        public IMongoDatabase Db { get; set; }
        public MongoClient Client { get; set; }



        public MongoDBClass(string database)
        {
            Client = new MongoClient(@"mongodb://10.64.199.115:27017");
            Db = Client.GetDatabase(database);
        }


        public void InsertRecord<T>(string Table, T Record)
        {
            var collection = Db.GetCollection<T>(Table);
            collection.InsertOne(Record);
        }
        public List<T> LoadRecord<T>(string Table)
        {
            var collection = Db.GetCollection<T>(Table);

            return collection.Find(new BsonDocument()).ToList();
        }
        public T LoadRecordById<T>(string Table, Guid Id)
        {
            var collection = Db.GetCollection<T>(Table);
            var filter = Builders<T>.Filter.Eq("Id", Id);

            return collection.Find(filter).First();
        }
        public T LoadRecordByName<T>(string Table, string Field, string Value)
        {
            var collection = Db.GetCollection<T>(Table);
            var filter = Builders<T>.Filter.Eq(Field, Value);
            try
            {
                var result = collection.Find(filter).FirstOrDefault();
                return result;
            }
            catch (System.InvalidOperationException e)
            {
                return default;
            }
        }
    }

}
