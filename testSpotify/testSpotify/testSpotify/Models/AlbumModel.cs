using System.Collections.Generic;

namespace testSpotify.Models
{
        public class AlbumModel
        {
            public string AlbumName { get; set; }
            public List<TrackModel> Tracks { get; set; }
        }
}
