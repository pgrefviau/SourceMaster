using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SourceMaster.Output;

namespace SourceMaster.Syntax
{
	public class GroupedSyntaxElement
	{
		public GroupedSyntaxElement(SyntaxElementKind kind, string text)
		{
			Kind = kind;
			Text = text;
		}

		public SyntaxElementKind Kind { get; }
		public string Text { get; }

		public override string ToString()
		{
			return Text;
		}
	}
}
