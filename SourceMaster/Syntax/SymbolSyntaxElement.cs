using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class SymbolSyntaxElement : CompositeSyntaxElement
	{
		public SymbolSyntaxElement(string trackingId, SyntaxElementKind kind, SyntaxToken token)
			: base(kind, token)
		{
			TrackingId = trackingId;
		}

		public SymbolSyntaxElement(string trackingId, SyntaxElementKind kind, string text) 
			: base(kind, text)
		{
			TrackingId = trackingId;
		}

		public string TrackingId { get; }
	}
}
