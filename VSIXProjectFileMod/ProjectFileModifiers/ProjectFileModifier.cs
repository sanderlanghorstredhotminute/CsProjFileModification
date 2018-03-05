using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using VSIXProjectFileMod.Equality;
using VSIXProjectFileMod.Helpers;

namespace VSIXProjectFileMod.ProjectFileModifiers
{
    public class ProjectFileModifier
    {
        #region Methods

        #region Static Methods
        
        /// <summary>
        /// Modifies the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static bool Modify(string filename)
        {
            if (File.Exists(filename))
            {
                var readerSettings = new XmlReaderSettings();

                var writerSettings = new XmlWriterSettings
                {
                    NewLineHandling = NewLineHandling.Replace,
                    NewLineChars = "\r\n",
                    CloseOutput = true,
                    Indent = true,
                    Encoding = Encoding.UTF8
                };

                XDocument xmlDocument;

                using (var reader = XmlReader.Create(filename, readerSettings))
                {
                    xmlDocument = XDocument.Load(reader);
                }
                //xmlDocument.Validate(readerSettings.Schemas, (sender, args) => Console.WriteLine(args.Message));

                //backup :)
                using (var writer = XmlWriter.Create($"{filename}.bak", writerSettings))
                {
                    xmlDocument.WriteTo(writer);
                }

                MendProjectDocument(xmlDocument);

                //xmlDocument.Validate(readerSettings.Schemas, (sender, eventArgs) => Console.WriteLine(eventArgs.Message));

                using (var writer = XmlWriter.Create(filename, writerSettings))
                {
                    xmlDocument.WriteTo(writer);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the main node grouping.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private static ElementGroupKey GetMainNodeGrouping(ElementWithRoot element)
        {
            switch (element.Root.Name.LocalName)
            {
                case "PropertyGroup":
                    return new ElementGroupKey {Element = element.Root, GroupKey = ""};
            }
            return new ElementGroupKey {Element = element.Root, GroupKey = element.Element?.Name.LocalName};
        }

        /// <summary>
        /// Gets the node order.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static string GetMainNodeOrder(XElement node)
        {
            /*
             * Sort By:
             * - Node name
             * - Attributes
             * - Children
             */
            var sbKey = new StringBuilder();
            switch (node.Name.LocalName)
            {
                case "Import":
                    sbKey.Append(1);
                    break;
                case "ProjectExtensions":
                    sbKey.Append(2);
                    break;
                case "PropertyGroup":
                    sbKey.Append(3);
                    break;
                case "ItemGroup":
                    sbKey.Append(4);
                    break;
                case "Target":
                    sbKey.Append(5);
                    break;
                default:
                    sbKey.Append(6);
                    break;
            }

            var attributes = node.Attributes().OrderBy(a => a.Name.LocalName).ToList();
            sbKey.Append(string.Join("", attributes.Select(a => a.Name.LocalName.Substring(0, 1))));
            sbKey.Append(string.Join("", attributes.Select(a => a.Value)));
            var subElements = node.Elements().Select(e => e.Name.LocalName.Substring(0, 1)).GroupBy(e => e).OrderBy(e => e.Count()).ToList();
            sbKey.Append(string.Join("", subElements.Select(s => s.Key)));

            var firstAttribute = node.Attributes().FirstOrDefault();
            if (firstAttribute != null)
            {
                sbKey.Append(firstAttribute.Value.GetHashCode());
            }

            return sbKey.ToString();
        }

        /// <summary>
        /// Gets the sub node order.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static string GetSubNodeOrder(XElement node)
        {
            var sbKey = new StringBuilder();
            sbKey.Append(node.Name.LocalName);
            var attrs = node.Attributes().OrderBy(a => a.Name.LocalName).ToList();
            sbKey.Append(string.Join("", attrs.Select(a => a.Name.LocalName.Substring(0, 1))));
            sbKey.Append(string.Join("", attrs.Select(a => a.Value)));

            return sbKey.ToString();
        }

        /// <summary>
        /// Sorts the project document.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        public static void MendProjectDocument(XDocument xmlDocument)
        {
            if (xmlDocument?.Root == null)
                return;

            //Sort
            var flatList = xmlDocument.Root.Elements().SelectMany(r => r.Elements().Any()
                                                                           ? r.Elements().Select(e => new ElementWithRoot {Root = r, Element = e})
                                                                           : new[] {new ElementWithRoot {Root = r, Element = null}}).ToList();
            var grouped = flatList.GroupBy(GetMainNodeGrouping);

            var newList = new List<XElement>();

            foreach (var elements in grouped)
            {
                var rootElement = new XElement(elements.Key.Element);
                rootElement.ReplaceNodes(elements.Select(e => e.Element).ToList());
                newList.Add(rootElement);
            }

            newList = newList.OrderBy(GetMainNodeOrder).ToList();

            foreach (var xElement in newList)
            {
                var subElements = xElement.Elements().OrderBy(GetSubNodeOrder).ToList();

                //dedupe
                subElements = subElements.Distinct(new XElementEqualtyComparer()).ToList();

                xElement.ReplaceNodes(subElements);
            }
            xmlDocument.Root.ReplaceNodes(newList);
        }

        #endregion

        #endregion

        #region Types

        private class ElementGroupKey
        {
            #region Fields

            public XElement Element;
            public string GroupKey;

            #endregion

            #region Methods

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((ElementGroupKey)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Element != null ? Element.ToEqualityString().GetHashCode() : 0) * 397) ^ (GroupKey != null ? GroupKey.GetHashCode() : 0);
                }
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            protected bool Equals(ElementGroupKey other)
            {
                return new XElementEqualtyComparer().Equals(Element, other.Element) && string.Equals(GroupKey, other.GroupKey);
            }

            #endregion
        }

        private class ElementWithRoot
        {
            #region Fields

            public XElement Element;
            public XElement Root;

            #endregion
        }

        #endregion
    }
}