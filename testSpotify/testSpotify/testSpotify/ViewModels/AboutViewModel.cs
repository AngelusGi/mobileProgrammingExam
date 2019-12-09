using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
namespace TestSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ICommand TestAuthCommand { get; }


        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command(() => TestAuth());
        }


        private async void TestAuth()
        {
            
        }
    }
}