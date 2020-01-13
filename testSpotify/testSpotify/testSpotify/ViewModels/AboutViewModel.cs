using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Plugin.Toast;
using MusixMatch_API;
using MusixMatch_API.APIMethods.Track;
using MusixMatch_API.ReturnTypes;
using MusixMatch_API.APIMethods.Artist;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MusixMatch_API.APIMethods.Matcher;

namespace testSpotify.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {

        private PlaybackContext playback;
        public ICommand TestAuthCommand { get; }

        public AboutViewModel()
        {
            Title = "About";
            TestAuthCommand = new Command(() => TestAuth());
        }

        private async void TestAuth()
        {
            playback = await SpotifyApi.GetPlaybackAsync();

            if (playback.Context != null || playback.Item != null)
            {
                Preferences.Set("LastDevice", playback.Device.Id);

                if (playback.IsPlaying)
                {
                    ErrorResponse x = await SpotifyApi.PausePlaybackAsync();
                }
                else
                {
                    ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(string.Empty, string.Empty,
                        new List<string>() { playback.Item.Uri }, "", playback.ProgressMs);
                }
            }
            else
            {
                var lastDevice = Preferences.Get("LastDevice", string.Empty);

                AvailabeDevices availabledevices = await SpotifyApi.GetDevicesAsync();
                SpotifyAPI.Web.Models.Device dev = null;

                availabledevices.Devices.ForEach(temp =>
                {
                    if (temp.Id == lastDevice)
                        dev = temp;
                });
                dev.IsActive = true;
                SpotifyApi.TransferPlayback(dev.Id);


                CursorPaging<PlayHistory> recentlyPlayed = await SpotifyApi.GetUsersRecentlyPlayedTracksAsync();
                ErrorResponse x = await SpotifyApi.ResumePlaybackAsync(
                    lastDevice, string.Empty,
                    new List<string>() { recentlyPlayed.Items.FirstOrDefault().Track.Uri }, string.Empty, 0);
            }
            //CrossToastPopUp.Current.ShowToastMessage(x.Error.Message);
        }
    }
}
