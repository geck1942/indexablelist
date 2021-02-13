using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polycorne.SiliconCity.Mechanics.GenericsEnhanced
{
	/// <summary>
	/// The class that depends on this Interface must return a getter (Key) that return a unique key from which an instance of this object will be indexed.
	/// </summary>
	/// <typeparam name="TKey">Type of the unique key of the object to index</typeparam>
	public interface IIndexable<TKey> 
		where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// How to read the key from this IIndexable item.
		/// </summary>
		TKey Key { get; }
	}
}
