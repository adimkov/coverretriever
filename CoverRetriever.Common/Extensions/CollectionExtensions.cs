// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Extensions to a collection
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace System.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Extensions to a collection.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Iterate each element in collection and apply an action.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="source">The collection.</param>
        /// <param name="action">Action to perform under collection item.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Add range into <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="source">The collection.</param>
        /// <param name="rangeForAdd">Elements for adding.</param>
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> rangeForAdd)
        {
            foreach (var item in rangeForAdd)
            {
                source.Add(item);
            }	
        }
    }
}