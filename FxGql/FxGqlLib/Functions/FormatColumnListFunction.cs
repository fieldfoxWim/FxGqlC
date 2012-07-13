using System;
using System.Linq;
using System.Collections.Generic;

namespace FxGqlLib
{
	public class FormatColumnListFunction : Expression<DataString>
	{
		readonly string separator;
		
		public FormatColumnListFunction (string separator)
		{
			this.separator = separator;
		}
				
		#region implemented abstract members of FxGqlLib.Expression[System.String]
		public override DataString Evaluate (GqlQueryState gqlQueryState)
		{
			IEnumerable<string> columns = gqlQueryState.Record.Columns.Select (p => p.ToString ());
			return Evaluate (columns);
		}
		#endregion

		public DataString Evaluate (IEnumerable<string> columns)
		{
			return new DataString (string.Concat (new SeparatorEnumerable<string> (columns, separator)));
		}

		class SeparatorEnumerable<T> : IEnumerable<T>
		{
			T separator;
			IEnumerable<T> enumerable;

			public SeparatorEnumerable (IEnumerable<T> enumerable, T separator)
			{
				this.separator = separator;
				this.enumerable = enumerable;
			}

			#region IEnumerable implementation
			public System.Collections.IEnumerator GetEnumerator ()
			{
				IEnumerator<T> enumerator = enumerable.GetEnumerator ();
				bool first = true;
				while (enumerator.MoveNext()) {
					if (first) 
						first = false;
					else
						yield return separator;
					yield return enumerator.Current;
				}
			}
			#endregion

			#region IEnumerable implementation
			IEnumerator<T> IEnumerable<T>.GetEnumerator ()
			{
				IEnumerator<T> enumerator = enumerable.GetEnumerator ();
				bool first = true;
				while (enumerator.MoveNext()) {
					if (first) 
						first = false;
					else
						yield return separator;
					yield return enumerator.Current;
				}
			}
			#endregion
		}
	}
}

