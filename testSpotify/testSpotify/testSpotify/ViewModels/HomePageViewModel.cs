using System;
using System.Collections.Generic;
using System.Text;
using testSpotify.Models;

namespace testSpotify.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private List<ArtistModel> artists;

        public List<ArtistModel> Artists
        {
            get { return artists; }
            set
            {
                artists = value; OnPropertyChanged();
            }
        }

        public HomePageViewModel()
        {
            Artists = App.Mongo.LoadRecord<ArtistModel>("Artist");
        }
    }
}
