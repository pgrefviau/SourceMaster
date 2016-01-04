using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SourceMaster.Semantic
{
	public class SymbolMetadata
	{
		public SymbolMetadata(string id)
		{
			Id = id;
		}

		public string Id { get; }

		public bool IsAvailableInSource { get; }
	}
}
