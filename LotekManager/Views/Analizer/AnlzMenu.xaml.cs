using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LotekManager.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LotekManager.Views.Analizer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AnlzMenu : ContentPage
	{
		public List<Item> Items { get; }
		public Command<Item> ItemTapped { get; }

		public AnlzMenu()
		{
			Items = new List<Item>()
			{
				new Item { Id = "a", Text = "Historia losowań", Description="Zobacz tabelę zawierającą wyniki." },
				new Item { Id = "b", Text = "Częstotliwość występowania", Description="Zobacz jak często wypadały poszczególne liczby." },
				new Item { Id = "c", Text = "Sprawdź swoje liczby", Description="Zobacz, czy kiedykolwiek wylosowano Twoje liczby." }
			};
			InitializeComponent();
			MenuListView.ItemsSource = Items;
			BindingContext = this;
			ItemTapped = new Command<Item>(OnItemSelected);
		}

		async void OnItemSelected(Item item)
		{
			if( item == null )
				return;

			if( item.Id == "a" )
				await Shell.Current.GoToAsync("/history");
			if( item.Id == "b" )
				await Shell.Current.GoToAsync("/heatmap");
			if( item.Id == "c" )
				await Shell.Current.GoToAsync("/checkNums");
		}
	}
}