using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Toolbox.Extension.Logic.Settings
{
    public class VsPersistSolutionOptions : IVsPersistSolutionOpts
    {
        private const string SettingsKey = "EfCoreToolboxSettingsKey";

        public int SaveUserOptions(IVsSolutionPersistence pPersistence)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            pPersistence.SavePackageUserOpts(this, SettingsKey);
            return VSConstants.S_OK;
        }

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                pPersistence.LoadPackageUserOpts(this, SettingsKey);
            }
            finally
            {
                Marshal.ReleaseComObject(pPersistence);
            }

            return VSConstants.S_OK;
        }

        public int WriteUserOptions(IStream pOptionsStream, string pszKey)
        {
            if (pszKey == SettingsKey)
            {

            }

            return VSConstants.S_OK;
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey)
        {
            if (pszKey == SettingsKey)
            {
                
            }

            return VSConstants.S_OK;
        }
    }
}
