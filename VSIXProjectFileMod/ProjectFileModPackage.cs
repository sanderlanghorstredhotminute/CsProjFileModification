using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;

namespace VSIXProjectFileMod
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPage), "Projectfile Modification", "Options", 115, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ProjectFileModPackage : Package
    {
        #region Fields

        /// <summary>
        /// Worker GUID string.
        /// </summary>
        public const string PackageGuidString = "cecf678d-88f2-42cb-aad1-2d1e3ebcdbbc";

        /// <summary>
        /// Gets a value indicating whether [autosave enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [autosave enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AutosaveEnabled
        {
            get
            {
                var page = (OptionPage)GetDialogPage(typeof(OptionPage));
                return page.Enabled;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            uint cookie;
            var runningDocumentTable = (IVsRunningDocumentTable)GetGlobalService(typeof(SVsRunningDocumentTable));

            runningDocumentTable.AdviseRunningDocTableEvents(new RunningDocTableEventsHandler(this, runningDocumentTable), out cookie);
            ProjectFileCommand.Initialize(this);
        }

        #endregion
    }
}