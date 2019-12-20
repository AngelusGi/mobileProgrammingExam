using MongoDB.Bson;
using System.Collections.Generic;

/* OAuth Token = BQA2JZJPBd3vAYqAplQZmXdxxR1NC*/

namespace testSpotify.Models
{
    public class ArtistModel
    {
        public ObjectId Id { get; set; }
        public string ArtistName { get; set; }
        public List<AlbumModel> Albums { get; set; }
    }
}
