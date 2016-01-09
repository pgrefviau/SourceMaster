using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceMaster.Extensions;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class SyntaxElement
	{
		public SyntaxElement(string text, SyntaxElementKind kind)
		{
			Kind = kind;
			Text = text;
		}

		public SyntaxElementKind Kind { get; }
		public string Text { get; }

		public virtual IEnumerable<SyntaxElement> CompositeElements => this.Yield();

		public override string ToString()
		{
			return $"{Kind}: {Text}";
		}
	}
}
