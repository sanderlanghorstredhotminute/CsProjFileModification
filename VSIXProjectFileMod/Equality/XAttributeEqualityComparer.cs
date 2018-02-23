using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VSIXProjectFileMod.Equality
{
    public class XAttributeEqualityComparer : IEqualityComparer<XAttribute>
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(XAttribute x, XAttribute y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Name.LocalName == y.Name.LocalName && x.Value == y.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(XAttribute obj)
        {
            return obj.Name.LocalName.GetHashCode() * obj.Value.GetHashCode();
        }

        #endregion
    }
}