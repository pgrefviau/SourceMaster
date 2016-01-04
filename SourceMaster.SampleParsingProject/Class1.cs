using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.SampleParsingProject
{
	public class Class1
	{
		public Class1()
		{
			if (true)
			{
				var foo = new Class2();
				var task = Task.Run(() => {});
			}
		}
	}
}
