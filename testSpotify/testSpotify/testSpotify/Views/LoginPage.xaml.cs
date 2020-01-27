using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Appearing += check;
        }
        private void check(object sender, EventArgs e)
        {
            List<Page> li = App.Current.MainPage.Navigation.NavigationStack.ToList();
            for (int i = li.Count - 2; i >= 0; --i)
            {
                App.Current.MainPage.Navigation.RemovePage(li.ElementAt(i));
            }
        }
    }
}