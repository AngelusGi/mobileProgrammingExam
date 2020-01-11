using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Linq;
using System.Net.Http;
using testSpotify.Utils;
using testSpotify.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testSpotify.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthView : ContentPage
    {
        private string absoluteurl;
        private AuthUtils authUtils;

        public AuthView()
        {
            InitializeComponent();
            authUtils = new AuthUtils();
            getAuth();
        }

        private void getAuth()
        {
            browser.Source = authUtils.ServerURI;

            browser.Navigating += async (object sender, WebNavigatingEventArgs e) =>
            {
                absoluteurl = e.Url;

                BaseViewModel.SpotifyApi = await authUtils.getApi(absoluteurl);

                if (BaseViewModel.SpotifyApi != null)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
            };
            //auth.OpenBrowser();
        }



        
    }
}