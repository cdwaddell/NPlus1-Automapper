using System.Collections.Generic;

namespace NPlus1_Automapper.Data
{
	public class BEntity
	{
		public BEntity()
		{
			Cs = new List<CEntity>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<CEntity> Cs { get; set; }
	}
}