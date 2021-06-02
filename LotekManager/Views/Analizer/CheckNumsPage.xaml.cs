using LotekManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views.Analizer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CheckNumsPage : ContentPage
	{
		public int[] EntriesVals;
		private List<DateTime>[] _hitsTable; // stores date where user hits 3 numbers, 4 numbers,... 6numbers
		public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
            nameof(ErrValidation),
            typeof(bool),
            typeof(CheckNumsPage),
            default(bool));
		public bool ErrValidation
		{ 
			get => (bool)GetValue(IsErrorProperty);
			set => SetValue(IsErrorProperty, value);
		}
		public CheckNumsPage()
		{
			InitializeComponent();
			ErrValidation = false;
			BindingContext = this;
			EntriesVals = new int[6];
			_hitsTable = new List<DateTime>[4];
			for( int i = 0; i < _hitsTable.Length; i++ ){
				_hitsTable[i] = new List<DateTime>();
			}
		}

		private void Entry_Unfocused(object sender, EventArgs e)
		{
			var text = ((Entry)sender).Text;
			var isParsed = int.TryParse(text, out int number);
			if(!isParsed)
				return;
			if( number < 1 || number > 49 )
			{
				// validation failed
				((Entry)sender).TextColor = Color.Red;
				ErrValidation = true;
				((Entry)sender).Focus();
			}
			else
			{
				// validation passed
				((Entry)sender).TextColor = Color.DarkSlateGray;
				ErrValidation = false;
				int idx = 0;
				foreach( Frame frm in stlEntries.Children )
				{
					if( frm.Content == sender )
					{
						EntriesVals[idx] = number;
						break;
					}
					else{
						idx++;
					}
				}

			}
		}

		private async void SrchButton_Clicked(object sender, EventArgs e)
		{
			if( ErrValidation ){
				return;
			}
			for( int i = 0; i < EntriesVals.Length; i++ )
			{
				if( EntriesVals[i] == 0 )
				{
					// Not all six numbers are given, show alert and return
					await DisplayAlert ("Wprowadź liczby!", "Nie wszystkie pola są uzupełnione", "OK");
					((Frame)stlEntries.Children.ElementAt(i)).Content.Focus();
					return;
				}
			}
			Array.Sort(EntriesVals);
			// Clear labels from previous values
			lblDt3.Text = ""; lblDt4.Text = "";
			lblDt5.Text = ""; lblDt6.Text = "";

			// Get data from db for last 60 days
			var today = DateTime.Today;
			var start = today.AddDays(-60);
			var lst = await App.DbConn.QueryAsync<Results>($"SELECT * FROM Results WHERE Date between '{start:yyyy-MM-dd}' and '{today:yyyy-MM-dd}' ");
			lblSadInfo.IsVisible = true;
			foreach( Results row in lst )
			{
				int matchedNums = CheckForWin(row);
				/* if user win somthing add date to position where e.g. _hitsTable[0] stores dates
				 * for case where user have three numbers matched */
				if( matchedNums >= 3 )
				{
					if( DateTime.TryParse(row.Date, out DateTime dtm) ) {
						_hitsTable[matchedNums - 3].Add(dtm);
					}
					lblSadInfo.IsVisible = false;
				}
			}
			lblTm3.Text = _hitsTable[0].Count.ToString();
			lblTm4.Text = _hitsTable[1].Count.ToString();
			lblTm5.Text = _hitsTable[2].Count.ToString();
			lblTm6.Text = _hitsTable[3].Count.ToString();
			foreach( var dt in _hitsTable[0] ){
				lblDt3.Text += dt.ToString("dd-MM-yyyy") + " ";
			}
			foreach( var dt in _hitsTable[1] ){
				lblDt4.Text += dt.ToString("dd-MM-yyyy") + " ";
			}
			foreach( var dt in _hitsTable[2] ){
				lblDt5.Text += dt.ToString("dd-MM-yyyy") + " ";
			}
			foreach( var dt in _hitsTable[3] ){
				lblDt6.Text += dt.ToString("dd-MM-yyyy") + " ";
			}
			for( int i = 0; i < _hitsTable.Length; i++ )
				_hitsTable[i].Clear();
			

		}
		private int CheckForWin(Results resultRow)
		{
			string temp;
			List<int> lstV = new List<int>();
			//var date = DateTime.Parse(resultRow.Date);
			// split values string from db row to list of this values
			for(int i = 0; i < resultRow.Vals.Length; i += 2)
			{
				temp = resultRow.Vals.Substring(i, 2);
				if( temp.StartsWith("0") )
					lstV.Add( int.Parse( temp.Substring(1) ));
				else
					lstV.Add(int.Parse( temp ));
			}
			int matchedNums = 0;
			for( int i = 0; i < EntriesVals.Length; i++ )
			{
				if( lstV.Contains(EntriesVals[i]) ){
					matchedNums++;
				}
			}
			return matchedNums;
		}
	}
}