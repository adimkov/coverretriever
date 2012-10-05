namespace CoverRetriever.AudioInfo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Object that can revert changes
    /// </summary>
    public abstract class EditableObject
    {
        /// <summary>
        /// Stored values.
        /// </summary>
        private readonly Dictionary<string, object> _beforeEditValues = new Dictionary<string, object>();

        /// <summary>
        /// id object currently in edit mode
        /// </summary>
        private bool _isEditing;
        
        /// <summary>
        /// Begins the edit.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (_isEditing)
            {
                return;
            }

            _isEditing = true;
            foreach (var property in GetWritableProperties())
            {
                _beforeEditValues.Add(property.Name, property.GetValue(this, null));
            }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public virtual void EndEdit()
        {
            _isEditing = false;
            _beforeEditValues.Clear();
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public virtual void CancelEdit()
        {
            _isEditing = false;
            var properties = GetWritableProperties().ToArray();
            foreach (var beforeEditValue in _beforeEditValues)
            {
                properties
                    .Single(x => x.Name == beforeEditValue.Key)
                    .SetValue(this, beforeEditValue.Value, null);
            }
        }

        /// <summary>
        /// Determines whether this instance is changed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChanged()
        {
            if (!_isEditing)
            {
                return false;
            }

            var isAllEqual = true;
            var properties = GetWritableProperties();

            foreach (var property in properties)
            {
                isAllEqual &= property.GetValue(this, null).Equals(_beforeEditValues[property.Name]);
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