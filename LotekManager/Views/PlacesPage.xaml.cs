using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlacesPage : ContentPage
	{
		public ObservableCollection<Models.Location> Locations { get; }
		public PlacesPage()
		{
			Locations = new ObservableCollection<Models.Location>();
			InitializeComponent();
			BindingContext = this;
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			Locations.Clear();
			// load saved locations from databse and add to view
			var lst = await App.DbConn.QueryAsync<Models.Location>($"SELECT * FROM Locations");
			foreach( var loc in lst )
			{
				//System.Diagnostics.Debug.WriteLine($"DEBUG: id- {loc.Id}, Adress- {loc.Adress}, lat- {loc.Latitude}, lon- {loc.Longitude}");
				Locations.Add(loc);
			}
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			LocationsListView.SelectedItems = null;
		}
		async void OnAddClicked(object sender, EventArgs e){
			await Shell.Current.GoToAsync(nameof(NewLocationPage));
		}
		private async void OnEditClicked(object sender, EventArgs e)
		{
			var selectedLocations = LocationsListView.SelectedItems;
			if( selectedLocations == null || selectedLocations.Count == 0  ) {
				await DisplayAlert ("Nie można edytować", "Wybierz jedną lokalizację z listy do edycji", "OK");
			}
			else {
				var loc = (Models.Location) selectedLocations.FirstOrDefault(); // if multiple locactions selected just get first
				await Shell.Current.GoToAsync( $"{nameof(LocationDetailPage)}?{nameof(LocationDetailPage.ItemId)}={loc.Id}" );
				LocationsListView.SelectedItems = null;
			}
		}

		private async void OnDeleteClicked(object sender, EventArgs e)
		{
			var selectedLocations = LocationsListView.SelectedItems;
			if( selectedLocations == null || selectedLocations.Count == 0  ) {
				await DisplayAlert ("Wybierz lokalizacje", "Wybierz jedną lub więcej lokalizacji, które chcesz usunąć", "OK");
				return;
			}
			bool answer = await DisplayAlert ("Uwaga!", "Czy na pewno chcesz trwale usunąć wybrane lokalizacje?", "Tak", "Nie");
			if( !answer ){
				return;
			}
			foreach( Models.Location slcloc in selectedLocations )
			{
				try
				{
					if ( slcloc.Id == null ){
						return;
					}
					var res = await App.DbConn.ExecuteAsync($"DELETE FROM Locations WHERE Id='{slcloc.Id}'");
				}
				catch( Exception ex )
				{
					System.Diagnostics.Debug.WriteLine($"Something is wrong: {ex.Message}");
					await DisplayAlert ("Ups :(", "Coś poszło nie tak, spróbuj jeszcze raz", "OK");
					return;
				}
				Locations.Remove(slcloc);
				LocationsListView.SelectedItems = null;
			}
		}

		private async void GoToMap_Tapped(object sender, EventArgs e)
		{
			MapPage.LocationsToDisp.Clear();
			foreach( Models.Location slcloc in LocationsListView.SelectedItems )
			{
				MapPage.LocationsToDisp.Add(slcloc);
			}
			await Shell.Current.GoToAsync(nameof(MapPage));
		}
	}
}