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
        static readonly string mongodbPath = @"mongodb://192.168.1.20:27017";
        static readonly string mongodbName = "LyricsfyTest";
        static UserPreferencesData database;
        static MongoDBClass mongo;

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
        public static MongoDBClass Mongo
        {
            get
            {
                if(mongo == null)
                {
                    mongo = new MongoDBClass(mongodbName,mongodbPath);
                }
                return mongo;
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
