using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SourceMaster.Output;
using SourceMaster.Semantic;

namespace SourceMaster.Syntax
{
	public class SymbolSyntaxElement : SyntaxElementWithTrivia
	{
		// TODO: Extract as extension method
		public SymbolSyntaxElement(SyntaxToken token, SymbolMetadata metadata)
			: base(token, SyntaxElementKind.Identifier)
		{
			TrackingId = metadata.Id;
			SymbolKind = metadata.SymbolKind;
			
		}

		public string TrackingId { get; }
		public SymbolKind SymbolKind { get; }
	}
}
