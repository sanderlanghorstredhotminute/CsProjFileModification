using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSIXProjectFileMod
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ProjectFileCommand
    {
        #region Fields

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9891e30d-b58a-47ee-a43e-c15953f3ab4e");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ProjectFileCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFileCommand" /> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ProjectFileCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = Instance.ServiceProvider.GetService(typeof(SDTE)) as DTE;
            if (dte.SelectedItems.Count <= 0) return;

            foreach (SelectedItem selectedItem in dte.SelectedItems)
            {
                if (selectedItem.ProjectItem == null) return;
                var projectItem = selectedItem.ProjectItem;
                var fullPathProperty = projectItem.Properties.Item("FullPath");
                if (fullPathProperty == null) return;
                var fullPath = fullPathProperty.Value.ToString();
                MessageBox.Show(string.Format("Required '{0}'.", fullPath));
            }
        }

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ProjectFileCommand(package);
        }

        #endregion

        #endregion
    }
}