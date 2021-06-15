
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    internal class ReferenceStore
    {
        /// <summary>
        /// Get reference by object
        /// </summary>
        private Dictionary<object, double> _to;

        /// <summary>
        /// Get object from reference
        /// </summary>
        private Dictionary<double, object> _from;

        internal ReferenceStore()
        {
            _to = new Dictionary<object, double>();
            _from = new Dictionary<double, object>();
        }

        /// <summary>
        /// Returns the object the given reference points to.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal object FollowReference(double reference)
        {
            return _from[reference];
        }

        /// <summary>
        /// Indicates wether this store contains a reference to the given object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal bool HasReferenceTo(object value)
        {
            return _to.ContainsKey(value);
        }

        /// <summary>
        /// Returns the reference value for the given object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal double GetReferenceTo(object value)
        {
            return _to[value];
        }

        /// <summary>
        /// Creates a reference for the given object.
        /// </summary>
        /// <param name="value"></param>
        internal void Reference(object value)
        {
            _to.Add(value, _to.Count);
            _from.Add(_from.Count, value);
        }
    }
}
