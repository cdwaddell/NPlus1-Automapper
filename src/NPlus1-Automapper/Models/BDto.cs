using System.Collections.Generic;

namespace NPlus1_Automapper.Models
{
	public class BDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<CDto> Cs { get; set; }
	}
}