﻿using SpotifyAPI.Web;
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
        private string _absoluteUrl;
        private AuthUtils _authUtils;

        public AuthView()
        {
            InitializeComponent();
            _authUtils = new AuthUtils();
            GetAuth();
        }

        private void GetAuth()
        {
            browser.Source = _authUtils.ServerUri;

            browser.Navigating += async (object sender, WebNavigatingEventArgs e) =>
            {
                _absoluteUrl = e.Url;

                BaseViewModel.SpotifyApi = await _authUtils.GetApi(_absoluteUrl);

                if (BaseViewModel.SpotifyApi != null)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
            };
            //auth.OpenBrowser();
        }
    }
}