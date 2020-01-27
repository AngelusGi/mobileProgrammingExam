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

        private void UpdateUI(object sender, EventArgs e)
        {
            check();
            BindingContext.GetType().GetMethod("UpdateUI").Invoke(BindingContext as HomePageViewModel, null);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DisplayAlert("", (e.Item as LocalModels.LocalArtistModel).Lyrics, "ok");
        }
        private void check()
        {
            List<Page> li = App.Current.MainPage.Navigation.NavigationStack.ToList();
            for (int i = li.Count - 2; i >= 0; --i)
            {
                App.Current.MainPage.Navigation.RemovePage(li.ElementAt(i));
            }
        }
    }
}