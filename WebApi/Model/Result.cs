using Microsoft.EntityFrameworkCore;
using System;
namespace LotekWebApi.Model
{
	[Keyless]
	public class Result
	{
		//public string rowid { get; set;}
		public string Date { get; set; }
		public string Vals { get; set; }
	}
}
