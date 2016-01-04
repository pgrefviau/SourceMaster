using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class CompositeSyntaxElement : GroupedSyntaxElement
	{
		public CompositeSyntaxElement(SyntaxElementKind kind, SyntaxToken token)
			: this(kind, token.ValueText)
		{
		}

		public CompositeSyntaxElement(SyntaxElementKind kind, string text)
			: base(kind, text)
		{
		}

		//public TextSpan Span { get; }

		public List<CompositeSyntaxElement> LeadingElements { get; } = new List<CompositeSyntaxElement>();
		public List<CompositeSyntaxElement> TrailingElements { get; } = new List<CompositeSyntaxElement>();
	}
}
