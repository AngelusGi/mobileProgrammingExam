using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using testSpotify.Services;
using testSpotify.Views;
using testSpotify.Models;
using testSpotify.DataBases;
using System.IO;

namespace testSpotify
{
    public partial class App : Application
    {
        static UserPreferencesData database;

        public static UserPreferencesData Database
        {
            get
            {
                if (database == null)
                {
                    database = new UserPreferencesData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserPreferences.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
