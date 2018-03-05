using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace VSIXProjectFileMod
{
    public class OptionPage : DialogPage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OptionPage" /> is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Projectfile Modification")]
        [DisplayName("Modify projectfile on save")]
        [Description("When this option is enabled, the projectfile modifier wil arrange the elements in the project file automatically. Visual studio will note that the file has changed outside the editor.")]
        public bool Enabled { get; set; }

        [Category("Projectfile Modification")]
        [DisplayName("Enabled Solutions")]
        [Description("Something something darkside")]
        public IDictionary<string, bool> EnabledSolutions { get; set; }

        #endregion
    }
}