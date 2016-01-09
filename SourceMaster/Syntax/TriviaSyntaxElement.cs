using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class TriviaSyntaxElement : SyntaxElement
	{
		public TriviaSyntaxElement(
			string text, 
			TriviaSyntaxElementKind triviaKind) 
			: base(text, SyntaxElementKind.Trivia)
		{
			TriviaKind = triviaKind;
		}

		public TriviaSyntaxElementKind TriviaKind { get; }
	}
}
