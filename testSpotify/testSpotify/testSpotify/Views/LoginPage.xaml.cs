using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testSpotify.Views.Spotify
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            //this.Appearing += check;
        }
        private async void check(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.InsertPageBefore(this, App.Current.MainPage.Navigation.NavigationStack.First());
            await App.Current.MainPage.Navigation.PopToRootAsync();
        }
    }
}