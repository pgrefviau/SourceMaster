using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Yield<T>(this T item)
		{
			yield return item;
		} 
	}
}
