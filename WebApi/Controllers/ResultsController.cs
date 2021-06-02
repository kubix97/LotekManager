using LotekWebApi.DAL;
using LotekWebApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotekWebApi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ResultsController : ControllerBase
	{
		private readonly AppDbContext _context;
		public ResultsController (AppDbContext context) {
            this._context = context;
        }

		[HttpGet("all")]
        public List<Result> Get(){
            return _context.Results.ToList(); 
        }
		[HttpGet("after")]
        public List<Result> Get(string date){

			var b = DateTime.TryParse(date, out DateTime parsedDate);
			if( !b ){
				return null;
			}
			return _context.Results.FromSqlRaw($"SELECT * FROM Results WHERE Date>'{parsedDate:yyyy-MM-dd}' ").ToList();
        }
	}
}
