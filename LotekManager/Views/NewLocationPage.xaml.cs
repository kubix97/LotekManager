using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewLocationPage : ContentPage
	{
		public NewLocationPage()
		{
			InitializeComponent();
		}
		protected override async void OnAppearing()
		{
			// get max id of location, incremtent and use it in the proposed name 
			var temp = await App.DbConn.QueryScalarsAsync<string>("SELECT MAX(Id) FROM Locations");
			int num = int.Parse(temp.FirstOrDefault()) + 1;
			entryName.Text = $"Lokalizacja {num}";

			try // get location of device
			{
				var location = await Geolocation.GetLastKnownLocationAsync();

				// if last known location is older than five mins, get new one
				var isOutdated =  DateTime.Compare(DateTime.Now.AddMinutes(-5), location.Timestamp.LocalDateTime);
				if( location == null || isOutdated > 0)
				{
					location = await Geolocation.GetLocationAsync(new GeolocationRequest()
					{ 
						DesiredAccuracy = GeolocationAccuracy.Medium,
						Timeout = TimeSpan.FromSeconds(30)
					});
				}
				if( location == null ){
					//System.Diagnostics.Debug.WriteLine("No location!");
					entryAdress.Text = "Gdzieś na Ziemi";
					lblLatitude.Text = "52.38574551767959";
					lblLongitude.Text = "19.36505485926635";
				}
				else
				{
					// save position in labels
					var culture = new CultureInfo("en-US");
					lblLatitude.Text = location.Latitude.ToString(culture);;
					lblLongitude.Text = location.Longitude.ToString(culture);

					// use obtained location to get possible address
					//var placemarks = await Geocoding.GetPlacemarksAsync(52.20246420000001, 21.02385264602882); // for DEBUG on simulator- to remove
					var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

					if( placemarks == null ){
						return;
					}
					Placemark placemark = null;
					foreach( Placemark p in placemarks )
					{
						if( p.Thoroughfare == null || p.Thoroughfare.Equals("Unnamed Road") ) {
							placemark = placemarks?.FirstOrDefault();
						}
						else {
							placemark = p;
							break;
						}
							
					} 
					var geocodeAddress =
						$"AdminArea:       {placemark.AdminArea}\n" + // wojewodztwo
						$"Locality:        {placemark.Locality}\n" + // miasto
						$"PostalCode:      {placemark.PostalCode}\n" +
						$"SubAdminArea:    {placemark.SubAdminArea}\n" + // gmina
						$"SubLocality:     {placemark.SubLocality}\n" + // dzielnica
						$"SubThoroughfare: {placemark.SubThoroughfare}\n" + // nr budynku
						$"Thoroughfare:    {placemark.Thoroughfare}\n";  // ulica

					//System.Diagnostics.Debug.WriteLine(geocodeAddress);
					entryAdress.Text = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}, {placemark.PostalCode} {placemark.Locality}";
				}
				

			}
			catch (FeatureNotSupportedException )
			{
				// Feature not supported on device
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine($"Something is wrong: {ex.Message}");
			}
		}
		private async void SaveBtn_Clicked(object sender, EventArgs e)
		{
			try
			{
				var res = await App.DbConn.ExecuteAsync($"INSERT INTO Locations (Name, Adress, Latitude, Longitude) " +
				$"VALUES ('{entryName.Text.Trim()}', '{entryAdress.Text.Trim()}', '{lblLatitude.Text}', '{lblLongitude.Text}')");
				if( res != 1 ){
				await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
				return;
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine($"Something is wrong: {ex.Message}");
				await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
				return;
			}
			Shell.Current.SendBackButtonPressed(); // after data being saved in db go back
		}
		private void CancelBtn_Clicked(object sender, EventArgs e){
			Shell.Current.SendBackButtonPressed();
		}
	}
}