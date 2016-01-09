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
		public SymbolMetadata(string id, string fullName, string[] filePathsOfDeclarations)
		{
			Id = id;
			FullName = fullName;
			DeclarationFilesPaths = filePathsOfDeclarations;
		}

		public string Id { get; }
		public string FullName { get; }
		public string[] DeclarationFilesPaths { get; }

		public bool IsAvailableInSource { get; }

		public override string ToString()
		{
			return FullName;
		}
	}
}
