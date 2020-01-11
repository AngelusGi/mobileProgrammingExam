using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace testSpotify.Utils
{
    public class AuthUtils
    {
        private Token lastToken;
        private Token _token;
        private SpotifyWebAPI _api;
        private HttpRequestMessage request;
        private HttpClient httpClient;
        private TokenSwapAuth auth;
        private string code;
        public string ServerURI { get; set; }

        public AuthUtils()
        {
            httpClient = new HttpClient();

            auth = new TokenSwapAuth("http://40.68.75.212:80/spotify/index.php", "http://40.68.75.212:80/", Scope.PlaylistReadPrivate | Scope.UserReadRecentlyPlayed | Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserReadCurrentlyPlaying | Scope.UserReadPlaybackState)
            {
                ShowDialog = false
            };
            auth.AuthReceived += async (sender, response2) =>
            {
                lastToken = await auth.ExchangeCodeAsync(response2.Code);
                _api = new SpotifyWebAPI()
                {
                    TokenType = lastToken.TokenType,
                    AccessToken = lastToken.AccessToken
                };
            };
            auth.OnAccessTokenExpired += async (sender, e) => _api.AccessToken = (await auth.RefreshAuthAsync(lastToken.RefreshToken)).AccessToken;
            ServerURI = auth.GetUri();
            auth.Start();
        }

        private async Task<Token> setToken(string code)
        {
            request = new HttpRequestMessage(new HttpMethod("POST"), "https://accounts.spotify.com/api/token");
            request.Headers.TryAddWithoutValidation("Authorization", "Basic MmU0NGE2NmIyMzYxNGFjOWIyYWFhMzFiNTI1ZGQxYjI6ODlkZTIwNWRlZDc3NGE0MWIxMzc4NTc4MDZjMWU0Nzk=");

            var contentList = new List<string>();
            contentList.Add("grant_type=authorization_code");
            contentList.Add(code);
            contentList.Add("redirect_uri=http%3A%2F%2F40.68.75.212%3A80%2Fspotify%2Fredirect.php");

            request.Content = new StringContent(string.Join("&", contentList));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Token>(data);
        }
        private string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public async Task<SpotifyWebAPI> getApi(string _absoultURL)
        {
            code = getBetween(_absoultURL, "?", "&");

            if (code.Contains("code="))
            {
                _token = await setToken(code);

                if (_token != null)
                {
                    _api = new SpotifyWebAPI()
                    {
                        TokenType = _token.TokenType,
                        AccessToken = _token.AccessToken,

                    };

                    if (_api.AccessToken.Length > 1)
                    {

                        return _api;
                    }
                }
            }
            return default;
        }

    }
}
