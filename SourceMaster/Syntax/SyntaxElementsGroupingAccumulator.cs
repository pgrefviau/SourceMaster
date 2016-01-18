using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class SyntaxElementsGroupingAccumulator
	{
		private readonly StringBuilder _literalGroupingAccumulator = new StringBuilder();
		private readonly List<SyntaxElement> _elements = new List<SyntaxElement>();

		public SyntaxElement[] GetResults()
		{
			FlushAccumulatedLitteralContent();
			return _elements.ToArray();
		}

		public void Add(params SyntaxElementWithTrivia[] elements)
		{
			var compositeElements = elements.SelectMany(elem => elem.CompositeElements);

			foreach (var element in compositeElements)
			{
				var isPurelyLitteral = IsPurelyLitteral(element);
				if (isPurelyLitteral)
				{
					// Accumulate
					_literalGroupingAccumulator.Append(element.Text);
				}
				else
				{
					// Flush accumulator
					FlushAccumulatedLitteralContent();

					// Add element
					_elements.Add(element);
				}
			}	
		}

		private void FlushAccumulatedLitteralContent()
		{
			if (_literalGroupingAccumulator.Length != 0)
			{
				var groupedLitteralElements = new SyntaxElement(_literalGroupingAccumulator.ToString(), SyntaxElementKind.Litteral);
				_elements.Add(groupedLitteralElements);

				_literalGroupingAccumulator.Clear();
			}
		}

		private bool IsPurelyLitteral(SyntaxElement element)
		{
			return
				element.Kind == SyntaxElementKind.Litteral || 
				(element as TriviaSyntaxElement)?.TriviaKind == TriviaSyntaxElementKind.Whitespace;
		}
	}
}
