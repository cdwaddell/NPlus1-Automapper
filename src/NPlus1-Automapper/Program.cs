using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NPlus1_Automapper.Data;
using NPlus1_Automapper.Models;

namespace NPlus1_Automapper
{
	public partial class Program
	{
		public static void Main(string[] args)
		{
			Mapper.Initialize(cfg =>
			{
				cfg.AddProfile(typeof(EntityProfile));
			});

			InitializeData();

			ADto aDto;
			using (var context = new MyDbContext())
			{
				context.ConfigureLogging();

				//using automapper
				//aDto = context.As.Where(a => a.Name == "Root")
				//	.Include(a => a.Bs).ThenInclude(b => b.Cs)
				//	.ProjectTo<ADto>()
				//	.SingleOrDefault();

				aDto = context.As.Include(a => a.Bs).ThenInclude(b => b.Cs)
					.Where(a => a.Name == "Root")
					.Select(a =>
						new ADto
						{
							Name = a.Name,
							Bs = a.Bs.Select(b =>
								new BDto
								{
									Name = b.Name,
									Id = b.Id,
									Cs = b.Cs.Select(c => new CDto
									{
										Name = c.Name,
										Id = c.Id
									}).ToList()
								}
							).ToList()
						}
					).SingleOrDefault();
			}

			Console.WriteLine(aDto.Name);
			Console.WriteLine("Press Any Key To Continue");

			Console.Read();
		}
	}

	public partial class MyDbContext : DbContext
	{
		public DbSet<AEntity> As { get; set; }
		public DbSet<BEntity> Bs { get; set; }
		public DbSet<CEntity> Cs { get; set; }
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
