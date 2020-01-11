using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Essentials;
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
        //private const string MongodbPath = @"mongodb://40.68.75.212:27017";

        private const string MongodbPath =
            "mongodb://unimol:MongoUnimol2020@40.68.75.212:27017/?authSource=admin&readPreference=primary&ssl=false";

        private const string MongodbName = "LyricsfyTest";
        private static UserPreferencesData _database;

        private static MongoDbClass _mongo;
        //private string _userName;


        //public static UserPreferencesData Database
        //{
        //    get
        //    {
        //        if (_database == null)
        //        {
        //            _database = new UserPreferencesData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserPreferences.db3"));
        //        }
        //        return _database;
        //    }
        //}
        public static UserPreferencesData Database =>
            _database ?? (_database = new UserPreferencesData(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "UserPreferences.db3")));


        //public static MongoDBClass Mongo
        //{
        //    get
        //    {
        //        if (_mongo == null)
        //        {
        //            _mongo = new MongoDBClass(MongodbName, MongodbPath);
        //        }
        //        return _mongo;
        //    }
        //}
        public static MongoDbClass Mongo => _mongo ?? (_mongo = new MongoDbClass(MongodbName, MongodbPath));


        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new AuthView())
            {
            };
            //CheckCredentials();
        }

        //private async void CheckCredentials()
        //{
        //    //Preferences.Remove(_userName);
        //    if (Preferences.Get(_userName, string.Empty).Equals(String.Empty))
        //    {

        //        if (await MainPage.DisplayAlert("Memorizza credenziali", "Vuoi memorizzare le credenziali?", "Si", "No"))
        //        {
        //            //todo _obtainedCredentials
        //            Preferences.Set(_userName, "test");
        //            Debug.Print($"{nameof(_userName)} = {_userName}");
        //        }
        //        else
        //        {
        //            Preferences.Remove(_userName);
        //            Debug.Print($"{nameof(_userName)} = {_userName}");

        //        }

        //    }
        //    else
        //    {
        //        //await MainPage.DisplayAlert("test", $"{nameof(_userName)} = {_userName}", "ok");
        //        await MainPage.DisplayAlert("test", $"{nameof(_userName)} = {Preferences.Get(_userName, string.Empty)}", "ok");

        //        //todo inserimento login

        //    }
        //}


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