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
        //private const string MongodbPath = @"mongodb://13.73.155.46:27017";
        private const string MongodbPath =
            "mongodb://unimol:MongoUnimol2020@13.73.155.46:27017/?authSource=admin&readPreference=primary&appname=MongoDB%20Compass%20Community&ssl=false";
        private const string MongodbName = "LyricsfyTest";
        private static UserPreferencesData _database;
        private static MongoDBClass _mongo;


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
        public static MongoDBClass Mongo => _mongo ?? (_mongo = new MongoDBClass(MongodbName, MongodbPath));


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
