using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Context
{
	public interface IContextualProperty<T>
	{
		T Value { get; }

		IContextualChange<T> PostValue(T value);
	}
}
