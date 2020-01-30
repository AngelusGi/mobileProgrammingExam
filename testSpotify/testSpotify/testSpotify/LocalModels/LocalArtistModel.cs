using MongoDB.Bson;
using SQLite;
using System.Collections.Generic;

/* OAuth Token = BQA2JZJPBd3vAYqAplQZmXdxxR1NC*/

namespace testSpotify.LocalModels
{
    public class LocalArtistModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string TrackName { get; set; }
        public string Lyrics { get; set; }
        public string AlbumImage { get; set; }
    }
}