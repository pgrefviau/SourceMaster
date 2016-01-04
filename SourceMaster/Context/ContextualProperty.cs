using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMaster.Context
{
	public class ContextualProperty<T> : IContextualProperty<T>
	{
		private readonly Stack<T> _values = new Stack<T>();

		public ContextualProperty()
		{
		}
		public ContextualProperty(T initialValue)
		{
			_values.Push(initialValue);
		}

		public T Value => _values.Any() ? _values.Peek() : default(T); 

		public IContextualChange<T> PostValue(T value)
		{
			_values.Push(value);

			return new ContextualChange<T>(
				value,
				() =>
				{
					if (_values.Any())
					{
						_values.Pop();
					}
					else
					{
					
					}
				});
		}
	}
}
