using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceMaster.Extensions;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class SyntaxElementWithTrivia : SyntaxElement
	{
		// TODO: Extract as extension method
		public SyntaxElementWithTrivia(SyntaxToken token, SyntaxElementKind kind)
			: this(token.ValueText, kind)
		{
		}

		public SyntaxElementWithTrivia(string text, SyntaxElementKind kind)
			: base(text, kind)
		{
		}

		//public TextSpan Span { get; }
		public override IEnumerable<SyntaxElement> CompositeElements => LeadingElements.Concat(base.CompositeElements).Concat(TrailingElements);

		private IEnumerable<TriviaSyntaxElement> AllTrivia => LeadingElements.Concat(TrailingElements);

		public IEnumerable<SyntaxElement> AllElements => LeadingElements.Concat<SyntaxElement>(this.Yield()).Concat(TrailingElements);

		public bool OnlyHasWhitespaceTrivia => AllTrivia.All(trivia => trivia.TriviaKind == TriviaSyntaxElementKind.Whitespace);

		public string FlattenedTextAndTrivia => string.Join(string.Empty, AllElements.Select(element => element.Text));

		//public bool CanFormLiteralGroupWith(SyntaxElementWithTrivia otherElement)
		//{
		//	return
		//		this.Kind == SyntaxElementKind.Litteral &&
		//		otherElement?.Kind == SyntaxElementKind.Litteral &&
		//		OnlyHasWhitespaceTrivia;
		//}

		public List<TriviaSyntaxElement> LeadingElements { get; } = new List<TriviaSyntaxElement>();
		public List<TriviaSyntaxElement> TrailingElements { get; } = new List<TriviaSyntaxElement>();
	}
}
