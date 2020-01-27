using System;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace testSpotify.Utils
{
    public class AuthUtils
    {
        private bool passed = false;
        private Token _lastToken;
        private Token _token;
        private SpotifyWebAPI _api;
        private HttpRequestMessage _request;
        private HttpClient _httpClient;
        private TokenSwapAuth _auth;
        private string _code;
        public string ServerUri { get; set; }

        public AuthUtils()
        {
            _httpClient = new HttpClient();
            _auth = new TokenSwapAuth("http://40.68.75.212:80/spotify/index.php", "http://40.68.75.212:80/",
                Scope.PlaylistReadPrivate | Scope.UserReadRecentlyPlayed | Scope.UserReadPrivate | Scope.AppRemoteControl |
                Scope.UserReadCurrentlyPlaying | Scope.UserReadPlaybackState | Scope.Streaming | Scope.UserModifyPlaybackState)
            {
                ShowDialog = !Preferences.Get("AutoLogin", false)
            };


            _auth.AuthReceived += async (sender, response) =>
            {
                _lastToken = await _auth.ExchangeCodeAsync(response.Code);
                _api = new SpotifyWebAPI()
                {
                    TokenType = _lastToken.TokenType,
                    AccessToken = _lastToken.AccessToken
                };
            };
            _auth.OnAccessTokenExpired += async (sender, e) =>
                _api.AccessToken = (await _auth.RefreshAuthAsync(_lastToken.RefreshToken)).AccessToken;



            ServerUri = _auth.GetUri();
            _auth.Start();
        }

        private async Task<Token> SetToken(string code)
        {
            _request = new HttpRequestMessage(new HttpMethod("POST"), "https://accounts.spotify.com/api/token");
            _request.Headers.TryAddWithoutValidation("Authorization",
                "Basic MmU0NGE2NmIyMzYxNGFjOWIyYWFhMzFiNTI1ZGQxYjI6ODlkZTIwNWRlZDc3NGE0MWIxMzc4NTc4MDZjMWU0Nzk=");

            var contentList = new List<string>();
            contentList.Add("grant_type=authorization_code");
            contentList.Add(code);
            contentList.Add("redirect_uri=http%3A%2F%2F40.68.75.212%3A80%2Fspotify%2Fredirect.php");

            _request.Content = new StringContent(string.Join("&", contentList));
            _request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await _httpClient.SendAsync(_request);
            string data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Token>(data);
        }

        private string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                //int start = strSource.IndexOf(strStart, 0) + strStart.Length;
                //int end = strSource.IndexOf(strEnd, start);
                int start = strSource.IndexOf(strStart, 0, StringComparison.InvariantCulture) + strStart.Length;
                int end = strSource.IndexOf(strEnd, start, StringComparison.InvariantCulture);
                return strSource.Substring(start, end - start);
            }
            else
            {
                return "";
            }
        }

        public async Task<SpotifyWebAPI> GetApi(string absoultUrl)
        {
            _code = GetBetween(absoultUrl, "?", "&");

            if (_code.Contains("code="))
            {
                if (!passed)
                {
                    passed = true;
                    _token = await SetToken(_code);
                }



                if (_token != null)
                {
                    _api = new SpotifyWebAPI()
                    {
                        TokenType = _token.TokenType,
                        AccessToken = _token.AccessToken,
                    };
                    return _api;

                    //CAUSAVA NULL POINTER INIZIALE
                    //if (_api.AccessToken.Length > 1)
                    //{
                    //    return _api;
                    //}
                }
            }

            return default;
        }
    }
}