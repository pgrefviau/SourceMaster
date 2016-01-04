using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Context
{
	public interface IContextualChange<T> : IDisposable
	{
		T InitialValue { get; }
		void TryRevert();
	}
}
