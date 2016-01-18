using System.Collections.Generic;
using SourceMaster.Output;

namespace SourceMaster.Web.Models
{
	public class FileSourceViewModel
	{
		 public SourceFileParsingInfo Content { get; set; }

		public Dictionary<string, string> CurrentDirectoryFileNamesToPaths { get; set; }
	}
}