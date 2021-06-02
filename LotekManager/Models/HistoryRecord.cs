using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LotekManager.Models
{
	public class HistoryRecord
	{
		// This class holds one row to be displayed at HistoryPage
		public string Date { get; set; }
		public List<string> Vals { get; set; }
		public HistoryRecord()
		{
			Date = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
			Vals = new List<string>(6)
			{
				"0", "0", "0", "0", "0", "0"
			};
		}
	}
}
