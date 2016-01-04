using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Context
{
	public class ContextualChange<T> : IContextualChange<T>
	{
		private readonly Action _applyRevert;
		private bool _canRevert = true;

		public T InitialValue { get; }

		public ContextualChange(T initialValue, Action applyRevert)
		{
			_applyRevert = applyRevert;
			InitialValue = initialValue;
		}

		public void TryRevert()
		{
			if (_canRevert)
			{
				_applyRevert();
				_canRevert = false;
			}
		}

		public void Dispose()
		{
			TryRevert();
		}
	}
}
