using MongoDB.Bson;
using System.Collections.Generic;

/* OAuth Token = BQA2JZJPBd3vAYqAplQZmXdxxR1NC*/

namespace testSpotify.LocalModels
{
    public class LocalArtistModel
    {
        public int ID { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string TrackName { get; set; }
        public string Lyrics { get; set; }
    }
}
