using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LotekManager.Models;

namespace LotekManager.Views.Analizer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HeatmapPage : ContentPage
	{
		TimeSpan _timeSpan;
		int[] _tblFreq; // store number of occurences - index is equal to (ball number - 1) 
		int _iNumOfRecivedRows = 0; // number of rows recived from DB between dates
		bool _bInitFlag;
		Color[] _tblHtmapColors; // store colors to be used in heatmap
		public HeatmapPage()
		{
			InitializeComponent();
			_bInitFlag = true;
			_tblFreq = new int[49];

			_tblHtmapColors = new Color[] { Color.FromHex("#FAFAFA"), Color.FromHex("#FFE0B2"),
				Color.FromHex("#FFCC80"), Color.FromHex("#FFB74D"), Color.FromHex("#FFA726"), 
				Color.FromHex("#FF9800"), Color.FromHex("#FB8C00"), Color.FromHex("#F57C00"),
				Color.FromHex("#EF6C00"), Color.FromHex("#E65100")};

			// setting date pickers and their constraints
			dtpEnd.MaximumDate = dtpEnd.Date = DateTime.Today;
			dtpStart.MaximumDate = dtpEnd.Date.AddDays(-14.0); // set minimum 14 days window
			dtpStart.Date = DateTime.Today.AddDays(-31.0);
			dtpEnd.MinimumDate = dtpStart.Date.AddDays(14.0); // minimum time window is 14 days
			dtpStart.MinimumDate = DateTime.Today.AddDays(-365.0); // maximum time window is 365 days
			_timeSpan = dtpEnd.Date - dtpStart.Date;
			lblDays.Text = (_timeSpan.Days + 1).ToString(); // adding one because 

			int num = 1;
			// adding 49 balls with numbers to gridView
			for(int row = 0; row < 7; row++ )
			{
				for(int col = 0; col < 7; col++ )
				{
					Ball b = new Ball(num);
					grdHeatmap.Children.Add(b.Frame, col, row);
					num++;
				}
			}
			_bInitFlag = false;
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			BindableLayout.SetItemsSource(stlLegend,_tblHtmapColors);
			await GetResAndOccurences(dtpStart.Date, dtpEnd.Date);
			SetBallsColors();
			lblRecvRows.Text = _iNumOfRecivedRows.ToString();

		}
		async void OnEndDateSelected(object sender, DateChangedEventArgs args)
		{
			if( _bInitFlag )
				return;
			if (args.NewDate.Equals(args.OldDate)){
				return;
			}
			
			dtpStart.MinimumDate = args.NewDate.AddDays(-365.0);
			dtpStart.MaximumDate = args.NewDate.AddDays(-14.0);
			_timeSpan = args.NewDate - dtpStart.Date;
			lblDays.Text = (_timeSpan.Days + 1).ToString();
			
			actInd.IsRunning = true;
			IsBusy = true;
			await GetResAndOccurences(dtpStart.Date, args.NewDate);
			SetBallsColors();
			IsBusy = false;
			actInd.IsRunning = false;
		}
		async void OnStartDateSelected(object sender, DateChangedEventArgs args)
		{
			if( _bInitFlag )
				return;
			if (args.NewDate.Equals(args.OldDate)){
				return;
			}
			dtpEnd.MinimumDate = args.NewDate.AddDays(14.0);
			_timeSpan = dtpEnd.Date - args.NewDate;
			lblDays.Text = (_timeSpan.Days + 1).ToString();

			actInd.IsRunning = true;
			IsBusy = true;
			await GetResAndOccurences(args.NewDate, dtpEnd.Date);
			SetBallsColors();
			IsBusy = false;
			actInd.IsRunning = false;
		}
		async Task GetResAndOccurences(DateTime start, DateTime end)
		{
			Array.Clear(_tblFreq, 0, _tblFreq.Length); // clear old data before populating with new one
			var lst = await App.DbConn.QueryAsync<Results>($"SELECT Vals FROM Results WHERE Date between '{start:yyyy-MM-dd}' and '{end:yyyy-MM-dd}' ");
			_iNumOfRecivedRows = lst.Count();
			lblRecvRows.Text = _iNumOfRecivedRows.ToString();
			if( _iNumOfRecivedRows == 0 ){
				return;
			}
			bool b;
			foreach( var row in lst )
			{
				for( int i = 0; i < row.Vals.Length; i += 2 )
				{
					// parse to int each ball numer form one result row and count it occurrence in table
					b = int.TryParse(row.Vals.Substring(i, 2), out int ballValue);

					if( !b || ballValue < 1 || ballValue > 49 ){
						//System.Diagnostics.Debug.WriteLine($"DEBUG: Failed to parse value!");
						continue;
					}
					_tblFreq[ballValue - 1]++; // increase occurance of selected ball
				}
			}
		}
		void SetBallsColors()
		{
			int minFreq = _tblFreq.Min(); int maxFreq = _tblFreq.Max();
			lblMin.Text = minFreq.ToString();
			if( maxFreq < 10 ){
				lblMax.Text = "9";
			}
			else{
				lblMax.Text = maxFreq.ToString();
			}
			float fInterval = ( maxFreq - minFreq ) <= 10 ? 1.0f : ( ( maxFreq - minFreq ) / _tblHtmapColors.Length );

			for( int i = 0; i < _tblFreq.Length; i++ ) // iteration through ball numbers
			{
				if( maxFreq < 10 )
				{
					grdHeatmap.Children.ElementAt(i).BackgroundColor = _tblHtmapColors[ _tblFreq[i] ];
				}
				else
				{
					for( int clrNum = 1; clrNum < _tblHtmapColors.Length + 1; clrNum++ ) // iteration through colors (stating from 1 to fulfill formula)
					{
						if( _tblFreq[i] <= minFreq + (int) Math.Floor(fInterval * clrNum) )
						{
							grdHeatmap.Children.ElementAt(i).BackgroundColor = _tblHtmapColors[clrNum-1]; // set heatmap ball color
							// colors are used like bins but they are not even spaced - e.g. if interval = 1.2 according to above incode formula,
							// first bin(color) will represent 1 occurrence and 2 occurrences (we get <= 2), but second color will be connected
							// with exatly 3 occurrences
							break;
						}
					}
				}
			}
		}
	}
}