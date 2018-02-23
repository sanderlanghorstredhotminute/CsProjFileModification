using System.Linq;
using System.Xml.Linq;

namespace VSIXProjectFileMod.Helpers
{
    public static class EqualityHelpers
    {
        #region Methods

        #region Static Methods

        /// <summary>
        /// To the equality string.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string ToEqualityString(this XElement element)
        {
            return string.Concat(element.Name.LocalName, string.Join("", element.Attributes().Select(a => string.Concat(a.Name.LocalName, a.Value.Trim()))));
        }

        #endregion

        #endregion
    }
}