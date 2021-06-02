using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotekManager.Models;
using LotekManager.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartPage : ContentPage
	{
		private string _LastDbdate; // stores info about currently displayed last lottery date
		public List<string> sBallValues;
		public StartPage()
		{
			InitializeComponent();
			sBallValues = new List<string>();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			//get the newest lottery result stroed in local databse
			RefreshViewData();
		}

		protected override void OnDisappearing()
		{
			sBallValues.Clear();
		}

		private async void OnRefresh_Clicked(object sender, EventArgs e)
		{
			if( Connectivity.NetworkAccess != NetworkAccess.Internet )
			{
				await DisplayAlert ("Nie można pobrać danych", "Brak dostępu do sieci", "OK");
				return;
			}
			var lst = await App.WebApi.GetDataAsync(_LastDbdate);

			if( lst.Count == 1 && lst[0].Date.StartsWith("@") )
			{
				await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
				return;
			}
			foreach( Results r in lst )
			{
				//System.Diagnostics.Debug.WriteLine($"DEBUG: Date: {r.Date}, Vals: {r.Vals}");
				if( r.Date != null && r.Vals != null )
				{
					var res = await App.DbConn.ExecuteAsync($"INSERT INTO Results (Date, Vals) VALUES ('{r.Date}', '{r.Vals}');");
					if( res < 1 )
					{
						await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
						return;
					}
				}
				else
				{
					await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
					return;
				}
				
			}
			RefreshViewData();
		}

		async void RefreshViewData()
		{
			sBallValues.Clear();
			var lst = await App.DbConn.QueryAsync<Results>("SELECT * FROM Results WHERE Date=(SELECT MAX(Date) FROM Results)");
			var res = lst[0];
			if( res != null)
			{
				_LastDbdate = res.Date;
				if( DateTime.TryParse(_LastDbdate, out DateTime dtLast) )
				{
					TimeSpan timeSpan = DateTime.Today - dtLast;
					if( timeSpan.Days > 4 ){
						lblValid.IsVisible = true;
					}
					else{
						lblValid.IsVisible = false;
					}
				}
				else{
					lblValid.IsVisible = false;
				}
				string temp;
				lblDate.Text = res.Date.Substring(8,2)+'.'+res.Date.Substring(5,2)+'.'+res.Date.Substring(0,4); //change from DB format to dd.MM.YYYY
				for(int i = 0; i < res.Vals.Length; i += 2)
				{
					temp = res.Vals.Substring(i, 2);
					if( temp.StartsWith("0") )
						sBallValues.Add( temp.Substring(1) );
					else
						sBallValues.Add(temp);
				}

			}
			BindableLayout.SetItemsSource(StlBalls,null);
			BindableLayout.SetItemsSource(StlBalls,sBallValues);
		}
	}
}