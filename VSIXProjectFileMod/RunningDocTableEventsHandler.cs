using System;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSIXProjectFileMod.ProjectFileModifiers;

namespace VSIXProjectFileMod
{
    internal class RunningDocTableEventsHandler : IVsRunningDocTableEvents3
    {
        #region Properties

        /// <summary>
        /// Gets the package.
        /// </summary>
        /// <value>
        /// The package.
        /// </value>
        private ProjectFileModPackage Package { get; }

        /// <summary>
        /// Gets or sets the running document table.
        /// </summary>
        /// <value>
        /// The running document table.
        /// </value>
        private IVsRunningDocumentTable RunningDocumentTable { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RunningDocTableEventsHandler" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="runningDocumentTable">The running document table.</param>
        public RunningDocTableEventsHandler(ProjectFileModPackage package, IVsRunningDocumentTable runningDocumentTable)
        {
            Package = package;
            RunningDocumentTable = runningDocumentTable;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called after application of the first lock of the specified type to a document in the Running Document Table (RDT).
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have been changed.</param>
        /// <param name="dwRDTLockType">[in] The document lock type. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop._VSRDTFLAGS" /> enumeration.</param>
        /// <param name="dwReadLocksRemaining">[in] Specifies the number of remaining read locks.</param>
        /// <param name="dwEditLocksRemaining">[in] Specifies the number of remaining edit locks.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called before releasing the last lock of the specified type on the specified document in the Running Document Table (RDT).
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have been changed.</param>
        /// <param name="dwRDTLockType">[in] Type of lock being released. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop._VSRDTFLAGS" /> enumeration.</param>
        /// <param name="dwReadLocksRemaining">[in] Specifies the number of remaining read locks.</param>
        /// <param name="dwEditLocksRemaining">[in] Specifies the number of remaining edit locks.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called after a document in the Running Document Table (RDT) is saved.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have been changed.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterSave(uint docCookie)
        {
            uint pgrfRDTFlags;
            uint pdwReadLocks;
            uint pdwEditLocks;
            string pbstrMkDocument;
            IVsHierarchy ppHier;
            uint pitemid;
            IntPtr ppunkDocData;
            RunningDocumentTable.GetDocumentInfo(docCookie, out pgrfRDTFlags, out pdwReadLocks, out pdwEditLocks, out pbstrMkDocument, out ppHier, out pitemid, out ppunkDocData);

            if (Package.AutosaveEnabled && pbstrMkDocument.EndsWith(".csproj"))
            {
                ProjectFileModifier.Modify(pbstrMkDocument);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called after an attribute of a document in the Running Document Table (RDT) changes.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have changed.</param>
        /// <param name="grfAttribs">[in] Flags corresponding to the changed attributes. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSRDTATTRIB" /> enumeration.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called before displaying a document window.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have been changed.</param>
        /// <param name="fFirstShow">[in] Non-zero (TRUE) if the doc window is being displayed for the first time.</param>
        /// <param name="pFrame">[in] The <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame" /> interface object representing the frame that contains the document's window.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called after a document window is hidden.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have been changed.</param>
        /// <param name="pFrame">[in] The <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame" /> interface representing the document window's frame.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called after a document attribute is changed. This is an advanced version of the <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsRunningDocTableEvents2.OnAfterAttributeChange(System.UInt32,System.UInt32)" /> method.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document whose attributes have changed.</param>
        /// <param name="grfAttribs">[in] Flags corresponding to the changed attributes. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSRDTATTRIB" /> enumeration.</param>
        /// <param name="pHierOld">[in] The <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface that previously owned the document.</param>
        /// <param name="itemidOld">
        /// [in] The previous item identifier. This is a unique identifier or it can be one of the following values: <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_NIL" />,
        /// <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT" />, or <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_SELECTION" />.
        /// </param>
        /// <param name="pszMkDocumentOld">[in] Name of the old document.</param>
        /// <param name="pHierNew">[in] The current <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface that owns the document.</param>
        /// <param name="itemidNew">
        /// [in] Indicates the new item identifier. This is a unique identifier or it can be one of the following values: <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_NIL" />,
        /// <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT" />, or <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_SELECTION" />.
        /// </param>
        /// <param name="pszMkDocumentNew">[in] Name of the new document.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called before saving a document.
        /// </summary>
        /// <param name="docCookie">[in] Abstract value representing the document about to be saved..</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}