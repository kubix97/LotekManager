using System;
using System.Collections.Generic;
using System.Text;

namespace LotekManager.Models
{
	/// <summary>
	/// Holds row from DB table named Locations
	/// </summary>
	public class Location
	{
		public string	Id			{ get; set; }
		public string	Name		{ get; set; }
		public string	Adress		{ get; set; }
		public string	Latitude	{ get; set; }
		public string	Longitude	{ get; set; }
	}
}
