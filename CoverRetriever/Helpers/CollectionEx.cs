using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq
{
	public static class CollectionEx
	{
		/// <summary>
		/// Iterate each element in collection and apply an action
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		/// <summary>
		/// Add range into <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="rangeForAdd"></param>
		public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> rangeForAdd)
		{
			foreach (var item in rangeForAdd)
			{
				source.Add(item);
			}	
		}
	}
}