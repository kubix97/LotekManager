using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[QueryProperty(nameof(ItemId), nameof(ItemId))]
	public partial class LocationDetailPage : ContentPage
	{
		private string _locationId;
		private Models.Location _edLocation;
		public string ItemId
		{
			get {
				return _locationId;
			}
			set {
				_locationId = value;
				LoadLocationById(value);
			}
		}
		public LocationDetailPage()
		{
			InitializeComponent();
		}
		async void LoadLocationById(string locationId)
		{
			//System.Diagnostics.Debug.WriteLine($"DEBUG: Otrzymane ID: {locationId}");
			var locs = await App.DbConn.QueryAsync<Models.Location>($"SELECT * FROM Locations WHERE Id='{locationId}'");
			_edLocation = locs.FirstOrDefault();
			entryName.Text = _edLocation.Name;
			entryAdress.Text = _edLocation.Adress;
			lblLatitude.Text = _edLocation.Latitude;
			lblLongitude.Text = _edLocation.Longitude;
		}
		private async void SaveBtn_Clicked(object sender, EventArgs e)
		{
			if( entryName.Text.Equals(_edLocation.Name) && entryAdress.Text.Equals(_edLocation.Adress) ){
				Shell.Current.SendBackButtonPressed(); // if nothing changed just go back
			}
			else
			{
				_edLocation.Name = entryName.Text.Trim();
				_edLocation.Adress = entryAdress.Text.Trim();
				var n = await App.DbConn.ExecuteAsync($"UPDATE Locations SET Name='{_edLocation.Name}', Adress='{_edLocation.Adress}' " +
					$"WHERE Id={_edLocation.Id} ");
				if( n != 1 ){
					await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
					return;
				}
				Shell.Current.SendBackButtonPressed(); // after data being saved in db go back
			}
		}
		private void CancelBtn_Clicked(object sender, EventArgs e){
			Shell.Current.SendBackButtonPressed();
		}
	}
}