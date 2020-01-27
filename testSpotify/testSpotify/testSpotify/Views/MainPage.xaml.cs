using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BottomBar.XamarinForms;
using testSpotify.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace testSpotify.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : BottomBarPage
    {
        private MainViewModel vm;
        public MainPage()
        {
            InitializeComponent();
            this.Appearing += check;
        }

        private void check(object sender, EventArgs e)
        {
            List<Page> li = App.Current.MainPage.Navigation.NavigationStack.ToList();
            for(int i = li.Count-2; i >= 0; --i)
            {
                App.Current.MainPage.Navigation.RemovePage(li.ElementAt(i));
            }
        }
    }
}