using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using LotekManager.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;

namespace LotekManager.Views.Analizer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : ContentPage
	{
		readonly SQLiteAsyncConnection _db;
		public List<HistoryRecord> HisRecords { get; set;} // List with binding to view
		public HistoryPage()
		{
			_db = App.DbConn;
			HisRecords = new List<HistoryRecord>();
			BindingContext = this;
			InitializeComponent();
			datePicker.MaximumDate = DateTime.Now;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			string date = datePicker.Date.ToString("yyyy-MM-dd");  // convert picker date format to DB date format
			var lst = await _db.QueryAsync<Results>($"SELECT * FROM Results WHERE Date<'{date}' ORDER BY Date DESC LIMIT 16");
			foreach( var v in lst)
				AddDbResultToView(v);

			// Fixing Xamarin bug #13678 described at: https://github.com/xamarin/Xamarin.Forms/pull/13678
			ResoultsListView.ItemsSource = null;
			ResoultsListView.ItemsSource = HisRecords;
		}
		protected override void OnDisappearing()
		{
			HisRecords.Clear();
		}
		async void OnDateSelected(object sender, DateChangedEventArgs args)
		{
			string sDtNew = args.NewDate.ToString("yyyy-MM-dd");
			var lst = await _db.QueryAsync<Results>($"SELECT * FROM Results WHERE Date<'{sDtNew}' ORDER BY Date DESC LIMIT 16");
			// clear old lottery results and add new
			HisRecords.Clear();
			foreach( var v in lst)
				AddDbResultToView(v);

			// Fixing Xamarin bug #13678 
			ResoultsListView.ItemsSource = null;
			ResoultsListView.ItemsSource = HisRecords;
		}
		/// <summary>
		/// Adding result row from DB to HisRecords list
		/// </summary>
		/// <param name="resultRow"></param>
		private void AddDbResultToView(Results resultRow)
		{
			string date = resultRow.Date;
			string temp;
			List<string> lstV = new List<string>();
			date = date.Substring(8,2)+'.'+date.Substring(5,2)+'.'+date.Substring(0,4); //change from DB format to dd.MM.YYYY
			for(int i = 0; i < resultRow.Vals.Length; i += 2)
			{
				temp = resultRow.Vals.Substring(i, 2);
				if( temp.StartsWith("0") )
					lstV.Add( temp.Substring(1) );
				else
					lstV.Add(temp);
			}
			HistoryRecord hisRcrd = new HistoryRecord { Date = date, Vals = lstV };
			HisRecords.Add(hisRcrd);
		}
	}
}