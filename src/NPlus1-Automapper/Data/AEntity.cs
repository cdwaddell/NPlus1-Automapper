using System.Collections.Generic;

namespace NPlus1_Automapper.Data
{
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
}