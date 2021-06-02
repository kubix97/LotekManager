using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LotekWebApi.Model;

namespace LotekWebApi.DAL
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions options) : base(options){
			}
		public DbSet<Result> Results { get; set; }
		
	}
}
