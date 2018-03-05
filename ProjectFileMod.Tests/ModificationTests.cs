using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSIXProjectFileMod.ProjectFileModifiers;

namespace ProjectFileMod.Tests
{
    [TestClass]
    public class ModificationTests
    {
        #region Properties

        /// <summary>
        /// Gets the different document.
        /// </summary>
        /// <value>
        /// The different document.
        /// </value>
        public XDocument DifferentDocument
        {
            get { return XDocument.Load(@"Resources/CodeGeneratorDifferent.xml"); }
        }

        /// <summary>
        /// Gets the formatted document.
        /// </summary>
        /// <value>
        /// The formatted document.
        /// </value>
        public XDocument FormattedDocument
        {
            get { return XDocument.Load(@"Resources/CodeGeneratorFormatted.xml"); }
        }

        /// <summary>
        /// Gets the unformatted document.
        /// </summary>
        /// <value>
        /// The unformatted document.
        /// </value>
        public XDocument UnformattedDocument
        {
            get { return XDocument.Load(@"Resources/CodeGeneratorUnformatted.xml"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Modifies the simple projectfile test deep equals formatted.
        /// </summary>
        [TestMethod]
        public void ModifySimpleProjectfileTest_DeepEquals_Formatted()
        {
            var document = UnformattedDocument;
            ProjectFileModifier.MendProjectDocument(document);
            Assert.IsTrue(XNode.DeepEquals(document, FormattedDocument), "The formatted file was different then expected");
        }

        /// <summary>
        /// Modifies the simple projectfile test deep equals not different.
        /// </summary>
        [TestMethod]
        public void ModifySimpleProjectfileTest_DeepEquals_Not_Different()
        {
            var document = UnformattedDocument;
            ProjectFileModifier.MendProjectDocument(document);
            Assert.IsFalse(XNode.DeepEquals(document, DifferentDocument), "The formatted file was different then expected");
        }

        /// <summary>
        /// Modifies the simple projectfile test deep equals not unformatted.
        /// </summary>
        [TestMethod]
        public void ModifySimpleProjectfileTest_DeepEquals_Not_Unformatted()
        {
            var document = UnformattedDocument;
            ProjectFileModifier.MendProjectDocument(document);
            Assert.IsFalse(XNode.DeepEquals(document, UnformattedDocument), "The formatted file was different then expected");
        }

        #endregion
    }
}