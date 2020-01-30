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
            return Update(artistModel) ? _database.UpdateAsync(artistModel) : _database.InsertAsync(artistModel);
        }

        private bool Update(LocalArtistModel artist)
        {
            List<LocalArtistModel> li = GetArtistsAsync().Result;
            bool found = false;

            li.ForEach(t =>
            {
                if (artist.TrackName.Equals(t.TrackName) && artist.ArtistName.Equals(t.ArtistName))
                    found = true;
            });

            return found;
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