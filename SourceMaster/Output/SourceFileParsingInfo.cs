using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceMaster.Syntax;

namespace SourceMaster.Output
{
	public class SourceFileParsingInfo
	{
		public SourceFileParsingInfo(string path, SyntaxElement[] elements)
		{
			Path = path;
			Elements = elements;
		}

		public string Path { get; }
		public SyntaxElement[] Elements { get; }
	}
}
