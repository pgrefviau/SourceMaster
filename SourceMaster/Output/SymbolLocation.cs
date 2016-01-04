using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Output
{
	public class SymbolLocation
	{
		public SymbolLocation(string filePath, string targetedId)
		{
			FilePath = filePath;
			TargetedId = targetedId;
		}

		public string FilePath { get; }
		public string TargetedId { get; }
	}
}
