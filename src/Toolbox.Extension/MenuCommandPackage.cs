using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Toolbox.Extension.Commands;
using Toolbox.Extension.Logic.Settings;
using Toolbox.Extension.Logic.Settings.Models;
using Task = System.Threading.Tasks.Task;

namespace Toolbox.Extension
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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(MenuCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class MenuCommandPackage : AsyncPackage, IVsPersistSolutionOpts
    {
        #region Solution related settings

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
                try
                {
                    using (var pStream = new DataStreamFromComStream(pOptionsStream))
                        if (pStream != null && pStream.CanWrite)
                        {
                            using (var ms = new MemoryStream())
                            {
                                var strValue = JsonConvert.SerializeObject(SettingsStore.Instance.MsSqlDatabaseConnectionSettings);
                                var data = Encoding.ASCII.GetBytes(strValue);
                                pStream.Write(data, 0, data.Length);
                                pStream.Flush();
                            }
                        }
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                }
            }

            return VSConstants.S_OK;
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey)
        {
            if (pszKey == SettingsKey)
            {
                try
                {
                    using (var pStream = new DataStreamFromComStream(pOptionsStream))
                        if (pStream != null && pStream.CanRead)
                        {
                            using (var ms = new MemoryStream())
                            {
                                byte[] data = new byte[pStream.Length];
                                pStream.Read(data, 0, data.Length);

                                ms.Write(data, 0, data.Length);
                                ms.Seek(0, SeekOrigin.Begin);
                                var strValue = Encoding.ASCII.GetString(data);
                                var msSqlConnectionSettings = JsonConvert.DeserializeObject<MsSqlDatabaseConnectionSettings>(strValue);

                                if (msSqlConnectionSettings != null)
                                    SettingsStore.SetInstance(msSqlConnectionSettings);
                            }
                        }
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                }
            }

            return VSConstants.S_OK;
        }

        #endregion


        /// <summary>
        /// MenuCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "97060ec1-eaab-47d4-a46e-66309cfde331";

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCommandPackage"/> class.
        /// </summary>
        public MenuCommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ScaffoldingMenuCommand.InitializeAsync(this);
            await AddMigrationMenuCommand.InitializeAsync(this);
            await ScriptMigrationMenuCommand.InitializeAsync(this);

            await base.InitializeAsync(cancellationToken, progress);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            ScaffoldingMenuCommand.Instance?.Dispose();
            AddMigrationMenuCommand.Instance?.Dispose();
            ScriptMigrationMenuCommand.Instance?.Dispose();

            base.Dispose(disposing);
        }
    }
}