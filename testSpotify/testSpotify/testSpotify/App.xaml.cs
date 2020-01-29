using System;
using System.IO;
using testSpotify.DataBases;
using testSpotify.Services;
using testSpotify.Views;
using testSpotify.Views.Spotify;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace testSpotify
{
    public partial class App : Application
    {
        private const string MongodbPath = 
            "mongodb://unimol:MongoUnimol2020@40.68.75.212:27017/?authSource=admin&readPreference=primary&ssl=false";

        private const string MongodbName = "LyricsfyTest";
        private static UserPreferencesData _database;

        private static MongoDbClass _mongo;

        public static UserPreferencesData Database =>
            _database ?? (_database = new UserPreferencesData(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "UserPreferences.db3")));

        public static MongoDbClass Mongo => _mongo ?? (_mongo = new MongoDbClass(MongodbName, MongodbPath));


        public App()
        {
            InitializeComponent();
            //DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new LoginPage());       
        }


        protected override void OnStart()
        {
            //Probabile Modo di gestire l'autologin
            //if (!Preferences.Get("AutoLogin", false))
            //    App.Current.MainPage = new NavigationPage(new AuthView());
        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {
            
        }
        
    }
}