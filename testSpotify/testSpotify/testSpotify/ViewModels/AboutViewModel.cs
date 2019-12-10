using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
namespace testSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command( () =>  TestAuth());
        }

        public ICommand TestAuthCommand { get; }

        private async void TestAuth()
        {
            
        }
    }
}