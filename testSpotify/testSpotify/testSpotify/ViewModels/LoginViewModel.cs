using System.Windows.Input;
using testSpotify.Views;
using Xamarin.Forms;

namespace testSpotify.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; set; }
        public ICommand ContinueCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(() => LoginCall());
            ContinueCommand = new Command(() => ContinueCall());
        }

        private void ContinueCall()
        {
            Logged = false;
            App.Current.MainPage = new MainPage();
        }

        private async void LoginCall()
        {
            Logged = true;
            await App.Current.MainPage.Navigation.PushModalAsync(new AuthView());
        }
    }
}
