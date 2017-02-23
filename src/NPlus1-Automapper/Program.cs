using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NPlus1_Automapper
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Mapper.Initialize(cfg =>
			{
				cfg.AddProfile(typeof(EntityProfile));
			});

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

			ADto aDto;
			using (var context = new MyDbContext())
			{
				var serviceProvider = context.GetInfrastructure();
				var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
				loggerFactory.AddProvider(new MyLoggerProvider());

				aDto = context.As.Where(a => a.Name == "Root").ProjectTo<ADto>().SingleOrDefault();
			}

			Console.WriteLine(aDto.Name);
			Console.WriteLine("Press Any Key To Continue");
			Console.Read();
		}
	}

	public class MyLoggerProvider : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName)
		{
			return new MyLogger();
		}

		public void Dispose()
		{ }

		private class MyLogger : ILogger
		{
			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				Console.WriteLine(formatter(state, exception));
			}

			public IDisposable BeginScope<TState>(TState state)
			{
				return null;
			}
		}
	}
	public class MyDbContext : DbContext
	{
		public DbSet<AEntity> As { get; set; }
		public DbSet<AEntity> Bs { get; set; }
		public DbSet<AEntity> Cs { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestAutomapper;Trusted_Connection=True;MultipleActiveResultSets=true");
			base.OnConfiguring(builder);
		}
	}

	public class AEntity
	{
		public AEntity()
		{
			Bs = new List<BEntity>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual ICollection<BEntity> Bs { get; set; }
	}

	public class BEntity
	{
		public BEntity()
		{
			Cs = new List<CEntity>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual ICollection<CEntity> Cs { get; set; }
	}

	public class CEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class ADto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual List<BDto> Bs { get; set; }
	}

	public class BDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual List<CDto> Cs { get; set; }
	}

	public class CDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class EntityProfile : Profile
	{
		public EntityProfile()
		{
			CreateMap<AEntity, ADto>();
			CreateMap<BEntity, BDto>();
			CreateMap<CEntity, CDto>();
		}
	}
}
