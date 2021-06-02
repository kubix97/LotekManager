using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using LotekManager.Models;
using Newtonsoft.Json;
using System.Threading;
using Xamarin.Essentials;

namespace LotekManager.Services
{
	public class WebApiService
	{
		HttpClient _client;
		string _ApiUrl = $"http://{App.AdressWbs}/Results/";
		public WebApiService()
		{
			_client = new HttpClient();
			
		}
		public async Task<List<Results>> GetDataAsync(string date)
		{
			Uri uri = new Uri(_ApiUrl+$"after?date={date:yyyy-MM-dd}");
			try
			{
				CancellationTokenSource	tks = new CancellationTokenSource(12 * 1000);
				HttpResponseMessage response = await _client.GetAsync(uri, tks.Token);
				//System.Diagnostics.Debug.WriteLine($"Resp get: {response.StatusCode}");
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					var resLst = JsonConvert.DeserializeObject<List<Results>>(content);
					return resLst;
				}
				return null;
			}
			catch(HttpRequestException ex) {
				List<Results>	lst = new List<Results>(1);
				lst.Add(new Results{Date = "@CN"});
				return lst;
			}
			catch(OperationCanceledException) {
				List<Results>	lst = new List<Results>(1);
				lst.Add(new Results{Date = "@TM"});
				return lst;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"DEBUG: {ex.Message}");
			}
			return null;
		}
		public async Task<List<Results>> GetDataAsync()
		{
			 Uri uri = new Uri(_ApiUrl+"all");
			HttpResponseMessage response = await _client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				var resLst = JsonConvert.DeserializeObject<List<Results>>(content);
				return resLst;
			}
			return null;
		}
	}
}
