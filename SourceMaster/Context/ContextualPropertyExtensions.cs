using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Context
{
	public static class ContextualPropertyExtensions
	{
		public static void ExecuteInContext<T>(
			this IContextualProperty<T> contextualProperty, 
			T valueInContext,
			Action<T> executeInContext)
		{
			using (contextualProperty.PostValue(valueInContext))
			{
				executeInContext(valueInContext);
			}
		}
	}
}
