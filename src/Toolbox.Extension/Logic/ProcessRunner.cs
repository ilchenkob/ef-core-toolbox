using Migrator.Logic.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace Toolbox.Extension.Logic
{
    internal class ProcessRunner
    {
        private IMigratorParams _currentParams;

        public Action<IMigratorParams, string> OutputDataCallback { get; set; }
        public Action<IMigratorParams, string> ErrorDataCallback { get; set; }

        public int Execute(IMigratorParams migratorParams)
        {
            _currentParams = migratorParams;

            try
            {
                using (var process = new Process())
                {
                    try
                    {
                        process.StartInfo = new ProcessStartInfo("dotnet")
                        {
                            Arguments = $"\"{Constants.MigratorAssembly}\" {migratorParams.ToArgumentString()}",
#if !DEBUG
                            WorkingDirectory = Path.GetDirectoryName(typeof(ProcessRunner).Assembly.Location),
#endif
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false
                        };

                        process.OutputDataReceived += onOutputDataReceived;
                        process.ErrorDataReceived += onErrorDataReceived;

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                    }
                    finally
                    {
                        process.OutputDataReceived -= onOutputDataReceived;
                        process.ErrorDataReceived -= onErrorDataReceived;
                    }

                    return process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                OutputDataCallback?.Invoke(_currentParams, ex.Message);
                return ExitCode.Exception;
            }
        }

        private void onOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args?.Data != null)
                OutputDataCallback?.Invoke(_currentParams, args.Data);
        }

        private void onErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args?.Data != null)
                ErrorDataCallback?.Invoke(_currentParams, args.Data);
        }
    }
}
