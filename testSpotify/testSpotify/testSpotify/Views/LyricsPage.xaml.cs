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
        private LyricsPageViewModel vm;
        public LyricsPage()
        {
            InitializeComponent();
            vm = new LyricsPageViewModel();
            this.BindingContext = vm;
            this.Appearing += LyricsPage_Appearing;
        }

        private void LyricsPage_Appearing(object sender, EventArgs e)
        {
            BindingContext.GetType().GetMethod("SetMatcherAsync").Invoke(vm, null);
        }
    }
}