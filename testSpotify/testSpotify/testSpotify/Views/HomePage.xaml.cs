using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSpotify.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testSpotify.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.Appearing += UpdateUI;
        }

        //Aggiungere al db sql lite l'update ed il controllo se un elemento esiste già
        //Aggiungere al db L'eliminazione di un elemento dal db


        private void UpdateUI(object sender, EventArgs e)
        {
            BindingContext.GetType().GetMethod("UpdateUI").Invoke(BindingContext as HomePageViewModel, null);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DisplayAlert((e.Item as LocalModels.LocalArtistModel).TrackName, (e.Item as LocalModels.LocalArtistModel).Lyrics, "OK");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var artist = button?.BindingContext as LocalModels.LocalArtistModel;
            var vm = BindingContext as HomePageViewModel;
            vm?.RemoveCommand.Execute(artist);
            BindingContext.GetType().GetMethod("UpdateUI").Invoke(BindingContext as HomePageViewModel, null);
        }
    }
}