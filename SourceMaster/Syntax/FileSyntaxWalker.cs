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
		private static readonly IContextualProperty<CompositeSyntaxElement> _currentSyntaxElement = new ContextualProperty<CompositeSyntaxElement>();
		
		public List<CompositeSyntaxElement> Elements { get; } = new List<CompositeSyntaxElement>();

		public FileSyntaxWalker(SemanticInterpreter semanticInterpreter)
			: base(SyntaxWalkerDepth.StructuredTrivia)
		{
			_semanticInterpreter = semanticInterpreter;
		}

		public override void VisitToken(SyntaxToken token)
		{
			CompositeSyntaxElement element;

			if (token.IsKeyword() || token.IsContextualKeyword())
			{
				// Keyword
				element = token.IsKind(SyntaxKind.PartialKeyword) 
					? ProcessPartialKeyword(token)
					: new CompositeSyntaxElement(SyntaxElementKind.Keyword, token);
			}
			else if (token.IsKind(SyntaxKind.IdentifierToken))
			{
				// Identifier
				
				element = ProcessSemanticSymbolIdentifier(token);
			}
			else if (IsStringLitteralToken(token))
			{
				// String litteral
				element = new CompositeSyntaxElement(SyntaxElementKind.StringLitteral, token);
			}
			else
			{
				// Litteral
				element = new CompositeSyntaxElement(SyntaxElementKind.Litteral, token);
			}

			using (_currentSyntaxElement.PostValue(element))
			{
				VisitLeadingTrivia(token);
				VisitTrailingTrivia(token);
			}

			Elements.Add(element);
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


		private CompositeSyntaxElement ProcessPartialKeyword(SyntaxToken partialKeywordToken)
		{
			return new CompositeSyntaxElement(SyntaxElementKind.Keyword, partialKeywordToken);
		}

		private CompositeSyntaxElement ProcessSemanticSymbolIdentifier(SyntaxToken identifierToken)
		{
			SymbolMetadata symbolMetadata;
			var parentSyntax = identifierToken.Parent;
			var isValidSymbol = _semanticInterpreter.EnsureSymbolMapping(parentSyntax, out symbolMetadata);

			return isValidSymbol 
				? new SymbolSyntaxElement(symbolMetadata.Id, SyntaxElementKind.Identifier, identifierToken) 
				: new CompositeSyntaxElement(SyntaxElementKind.Litteral, identifierToken);
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
			var targetedElements = isLeadingTrivia
				? currentElement.LeadingElements
				: currentElement.TrailingElements;

			switch (trivia.Kind())
			{
				case SyntaxKind.WhitespaceTrivia:
					ProcessWhitespaceTrivia(trivia, targetedElements, isLeadingTrivia);
					break;
				case SyntaxKind.EndOfLineTrivia:
					ProcessEndOfLineTrivia(trivia, targetedElements);
					break;
				case SyntaxKind.SingleLineCommentTrivia:
					ProcessCommentTrivia(trivia, targetedElements, isSingleLine: true);
					break;
			}

			base.VisitTrivia(trivia);
		}

		private void ProcessWhitespaceTrivia(SyntaxTrivia trivia, List<CompositeSyntaxElement> targetedElements, bool isLeadingTrivia)
		{
			var indentation = trivia.Span.End - trivia.Span.Start;
			var whitespaceCharacterCount = isLeadingTrivia 
				? indentation * WhitespaceCharacterPerLeadingIndentation 
				: indentation;

			targetedElements.Add(new CompositeSyntaxElement(SyntaxElementKind.Litteral, new string(' ', whitespaceCharacterCount)));
		}

		private void ProcessEndOfLineTrivia(SyntaxTrivia trivia, List<CompositeSyntaxElement> targetedElements)
		{
			targetedElements.Add(new CompositeSyntaxElement(SyntaxElementKind.Litteral, trivia.ToString()));
		}

		private void ProcessCommentTrivia(SyntaxTrivia trivia, List<CompositeSyntaxElement> targetedElements, bool isSingleLine)
		{
			targetedElements.Add(new CompositeSyntaxElement(SyntaxElementKind.Comment, trivia.ToString()));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			base.VisitDocumentationCommentTrivia(node);
		}
	}
}
