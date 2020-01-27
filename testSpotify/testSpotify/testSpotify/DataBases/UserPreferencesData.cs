using MongoDB.Bson;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testSpotify.LocalModels;

namespace testSpotify.DataBases
{
    public class UserPreferencesData
    {
        private readonly SQLiteAsyncConnection _database;

        public UserPreferencesData(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocalArtistModel>().Wait();
        }

        public Task<List<LocalArtistModel>> GetArtistsAsync()
        {
            return _database.Table<LocalArtistModel>().ToListAsync();
        }

        public Task<LocalArtistModel> GetArtistAsync(int id)
        {
            return _database.Table<LocalArtistModel>()
                .Where(artistModel => artistModel.ID == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveArtistAsync(LocalArtistModel artistModel)
        {
            //if(artistModel.ID != 0)
            //{
            //    return _database.UpdateAsync(artistModel);
            //}
            //else
            //{
            //    return _database.InsertAsync(artistModel);
            //}
            return artistModel.ID != 0 ? _database.UpdateAsync(artistModel) : _database.InsertAsync(artistModel);
        }

        public Task<int> DeleteArtistAsync(LocalArtistModel artistModel)
        {
            return _database.DeleteAsync(artistModel);
        }

        public async void DropDatabase()
        {
            await _database.DropTableAsync<LocalArtistModel>();
        }
    }
}