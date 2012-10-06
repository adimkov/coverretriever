// -------------------------------------------------------------------------------------------------
// <copyright file="EditableObject.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.  
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Object that can revert changes.
    /// </summary>
    public abstract class EditableObject
    {
        /// <summary>
        /// Stored values.
        /// </summary>
        private readonly Dictionary<string, object> beforeEditValues = new Dictionary<string, object>();

        /// <summary>
        /// Is object currently in edit mode.
        /// </summary>
        private bool isEditing;
        
        /// <summary>
        /// Begins the edit.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (isEditing)
            {
                return;
            }

            this.isEditing = true;
            foreach (var property in GetWritableProperties())
            {
                beforeEditValues.Add(property.Name, property.GetValue(this, null));
            }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public virtual void EndEdit()
        {
            this.isEditing = false;
            this.beforeEditValues.Clear();
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public virtual void CancelEdit()
        {
            this.isEditing = false;
            var properties = GetWritableProperties().ToArray();
            foreach (var beforeEditValue in this.beforeEditValues)
            {
                properties
                    .Single(x => x.Name == beforeEditValue.Key)
                    .SetValue(this, beforeEditValue.Value, null);
            }
            
            beforeEditValues.Clear();
        }

        /// <summary>
        /// Determines whether this instance is changed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChanged()
        {
            if (!this.isEditing)
            {
                return false;
            }

            var isAllEqual = true;
            var properties = GetWritableProperties();

            foreach (var property in properties)
            {
                isAllEqual &= property.GetValue(this, null).Equals(this.beforeEditValues[property.Name]);
            }

            return !isAllEqual;
        }

        /// <summary>
        /// Gets the writable properties.
        /// </summary>
        /// <returns>Writable objects.</returns>
        private IEnumerable<PropertyInfo> GetWritableProperties()
        {
            return GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanWrite);
        }
    }
}