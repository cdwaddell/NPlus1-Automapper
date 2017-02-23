using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPlus1_Automapper.Data;
using NPlus1_Automapper.Logging;

namespace NPlus1_Automapper
{
    public partial class Program
    {
		public static void InitializeData()
		{
			using (var context = new MyDbContext())
			{
				context.Database.EnsureCreated();

				var existingAs = context.As.Include(x => x.Bs).ThenInclude(x => x.Cs).ToList();
				if (existingAs.Any())
				{
					context.As.RemoveRange(existingAs);
					context.SaveChanges();
				}

				var a = new AEntity { Name = "Root" };
				for (var i = 0; i < 10; i++)
				{
					var b = new BEntity { Name = $"Item {i}" };
					a.Bs.Add(b);
					for (var j = 0; j < 20; j++)
					{
						b.Cs.Add(new CEntity { Name = $"Item {i}.{j}" });
					}
				}
				context.As.Add(a);
				context.SaveChanges();
			}
		}
	}

	public partial class MyDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestAutomapper;Trusted_Connection=True;MultipleActiveResultSets=true");
			
			base.OnConfiguring(builder);
		}

		public void ConfigureLogging()
		{
			var serviceProvider = this.GetInfrastructure();
			var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
			loggerFactory.AddProvider(new MyLoggerProvider());
		}
	}
}
