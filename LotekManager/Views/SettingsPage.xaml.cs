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
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();
			entryAdr.Text = App.AdressWbs;
		}

		private void OnSave_Clicked(object sender, EventArgs e)
		{
			App.AdressWbs = entryAdr.Text.Trim();
		}
	}
}