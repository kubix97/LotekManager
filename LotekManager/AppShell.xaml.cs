using LotekManager.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LotekManager
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			// added code
			Routing.RegisterRoute(nameof(LocationDetailPage), typeof(LocationDetailPage));
			Routing.RegisterRoute(nameof(NewLocationPage), typeof(NewLocationPage));
			Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
			Routing.RegisterRoute("AnlzMenu/history", typeof(Views.Analizer.HistoryPage));
			Routing.RegisterRoute("AnlzMenu/heatmap", typeof(Views.Analizer.HeatmapPage));
			Routing.RegisterRoute("AnlzMenu/checkNums", typeof(Views.Analizer.CheckNumsPage));
		}
	}
}
