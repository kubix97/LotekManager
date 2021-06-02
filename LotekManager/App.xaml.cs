using LotekManager.Services;
using LotekManager.Views;
using System;
using Xamarin.Forms;
using SQLite;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace LotekManager
{
	public partial class App : Application
	{
		static SQLiteAsyncConnection _dbConn;
		public static SQLiteAsyncConnection DbConn
		{
			get 
			{
				if( _dbConn == null ) {
					_dbConn = new SQLiteAsyncConnection( Path.Combine(FileSystem.AppDataDirectory, "LotekDB.db") );
				}
				return _dbConn;
					
			}
		}
		public static WebApiService WebApi;
		public static string AdressWbs= "192.168.0.39:5000";
		public App()
		{
			InitializeComponent();
			WebApi = new WebApiService();
			MainPage = new AppShell();
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
