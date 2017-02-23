using System.Collections.Generic;

namespace NPlus1_Automapper.Models
{
	public class ADto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual List<BDto> Bs { get; set; }
	}
}