#define lololol

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMaster.Context;
using SourceMaster.Output;
using SourceMaster.Semantic;

namespace SourceMaster.Syntax
{
	public class FileSyntaxWalker : CSharpSyntaxWalker
	{
		private const int WhitespaceCharacterPerLeadingIndentation = 4;

		private readonly SemanticInterpreter _semanticInterpreter;

		private static readonly IContextualProperty<bool> _isLeadingTrivia = new ContextualProperty<bool>(true);
		private static readonly IContextualProperty<SyntaxElementWithTrivia> _currentSyntaxElement = new ContextualProperty<SyntaxElementWithTrivia>();

		public SyntaxElementsGroupingAccumulator ResultsAccumulator { get; } = new SyntaxElementsGroupingAccumulator();

		public FileSyntaxWalker(SemanticInterpreter semanticInterpreter)
			: base(SyntaxWalkerDepth.StructuredTrivia)
		{
			_semanticInterpreter = semanticInterpreter;
		}

		public override void VisitToken(SyntaxToken token)
		{
			SyntaxElementWithTrivia element;

			if (token.IsKeyword() || token.IsContextualKeyword())
			{
				// Keyword
				element = token.IsKind(SyntaxKind.PartialKeyword) 
					? ProcessPartialKeyword(token)
					: new SyntaxElementWithTrivia(token, SyntaxElementKind.Keyword);
			}
			else if (token.IsKind(SyntaxKind.IdentifierToken))
			{
				// Identifier
				
				element = ProcessSemanticSymbolIdentifier(token);
			}
			else if (IsStringLitteralToken(token))
			{
				// String litteral
				element = new SyntaxElementWithTrivia(token, SyntaxElementKind.StringLitteral);
			}
			else
			{
				// Litteral
				element = new SyntaxElementWithTrivia(token, SyntaxElementKind.Litteral);
			}

			using (_currentSyntaxElement.PostValue(element))
			{
				VisitLeadingTrivia(token);
				VisitTrailingTrivia(token);
			}

			ResultsAccumulator.Add(element);
		}

		private bool IsStringLitteralToken(SyntaxToken token)
		{
			switch (token.Kind())
			{
				case SyntaxKind.StringLiteralToken:
				case SyntaxKind.InterpolatedStringTextToken:
					return true;
			}

			return false;
		}

		private SyntaxElementWithTrivia ProcessPartialKeyword(SyntaxToken partialKeywordToken)
		{
			// TODO: Special case handling
			return new SyntaxElementWithTrivia(partialKeywordToken, SyntaxElementKind.Keyword);
		}

		private SyntaxElementWithTrivia ProcessSemanticSymbolIdentifier(SyntaxToken identifierToken)
		{
			SymbolMetadata symbolMetadata;
			var parentSyntax = identifierToken.Parent;
			var isValidSourceSymbol = _semanticInterpreter.TryEnsuringSymbolMapping(parentSyntax, out symbolMetadata);

			return isValidSourceSymbol 
				? new SymbolSyntaxElement(identifierToken, symbolMetadata.Id) 
				: new SyntaxElementWithTrivia(identifierToken, SyntaxElementKind.Litteral);
		}

		public override void VisitLeadingTrivia(SyntaxToken token)
		{
			using (_isLeadingTrivia.PostValue(true))
			{
				base.VisitLeadingTrivia(token);
			}
		}

		public override void VisitTrailingTrivia(SyntaxToken token)
		{
			using (_isLeadingTrivia.PostValue(false))
			{
				base.VisitTrailingTrivia(token);
			}
		}

		public override void VisitTrivia(SyntaxTrivia trivia)
		{
			var currentElement = _currentSyntaxElement.Value;
			var isLeadingTrivia = _isLeadingTrivia.Value;
			var triviaText = trivia.ToString();

			var targetedElements = isLeadingTrivia
				? currentElement.LeadingElements
				: currentElement.TrailingElements;

			switch (trivia.Kind())
			{
				case SyntaxKind.EndOfLineTrivia:
				case SyntaxKind.WhitespaceTrivia:
					targetedElements.Add(new TriviaSyntaxElement(triviaText, TriviaSyntaxElementKind.Whitespace));
					break;
				case SyntaxKind.SingleLineCommentTrivia:
					// TODO: Add handling for other comments types
					ProcessCommentTrivia(trivia, targetedElements, isSingleLine: true);
					break;
			}

			base.VisitTrivia(trivia);
		}

		private void ProcessCommentTrivia(SyntaxTrivia trivia, List<TriviaSyntaxElement> targetedElements, bool isSingleLine)
		{
			targetedElements.Add(new CommentTriviaSyntaxElement(trivia.ToString()));
		}

		public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			return;
			//base.VisitDocumentationCommentTrivia(node);
		}
	}
}
