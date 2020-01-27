using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSpotify.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testSpotify.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LyricsPage : ContentPage
    {
        public LyricsPage()
        {
            InitializeComponent();
            this.Appearing += LyricsPage_Appearing;
            this.Appearing += check;
        }

        private void LyricsPage_Appearing(object sender, EventArgs e)
        {
            BindingContext.GetType().GetMethod("SetMatcherAsync").Invoke((BindingContext as LyricsPageViewModel), null);
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