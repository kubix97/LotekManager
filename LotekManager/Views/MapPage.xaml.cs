using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Linq;
using System.Globalization;

namespace LotekManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPage : ContentPage
	{
		public static List<Models.Location> LocationsToDisp = new List<Models.Location>();
		public MapPage()
		{
			InitializeComponent();
			// Get first location to center map on it
			var fstLocation = LocationsToDisp.FirstOrDefault();
			var culture = new CultureInfo("en-US"); // we store latitude and longitude in en-US format 
			if( fstLocation != null )
			{
				var bLat = double.TryParse(fstLocation.Latitude, NumberStyles.Any, culture, out double lat);
				var bLon = double.TryParse(fstLocation.Longitude, NumberStyles.Any, culture, out double lon);
				if ( bLat || bLon )
				{
					Position pos = new Position( lat, lon);
					MapSpan mapSpan = new MapSpan( pos, 0.01, 0.01);
					map.MoveToRegion(mapSpan);
				}
			}
			// add pins to map from given locations
			foreach( var l in LocationsToDisp )
			{
				var bLat = double.TryParse(l.Latitude, NumberStyles.Any, culture, out double lat);
				var bLon = double.TryParse(l.Longitude, NumberStyles.Any, culture, out double lon);
				if ( bLat || bLon )
				{
					Pin pin = new Pin { Label = l.Name, Position = new Position( lat , lon),
					Type = PinType.SavedPin, Address = l.Adress};
					map.Pins.Add(pin);
				}
			}
		}
	}
}