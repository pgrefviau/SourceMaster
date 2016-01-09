using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class SymbolSyntaxElement : SyntaxElementWithTrivia
	{
		// TODO: Extract as extension method
		public SymbolSyntaxElement(SyntaxToken token, string trackingId)
			: base(token, SyntaxElementKind.Identifier)
		{
			TrackingId = trackingId;
		}

		public SymbolSyntaxElement(string text, string trackingId) 
			: base(text, SyntaxElementKind.Identifier)
		{
			TrackingId = trackingId;
		}

		public string TrackingId { get; }
	}
}
