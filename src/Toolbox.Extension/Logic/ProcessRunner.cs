using Migrator.Logic.Models;
using System;
using System.Diagnostics;

namespace Toolbox.Extension.Logic
{
    internal class ProcessRunner
    {
        public Action<string> OutputDataCallback { get; set; }
        public Action<string> ErrorDataCallback { get; set; }

        public int Execute(MigratorParams migratorParams)
        {
            using (var process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo("dotnet")
                    {
                        Arguments = $"{Constants.MigratorAssembly} {migratorParams.ToArgumentString()}",
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

        private void onOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args?.Data != null)
            OutputDataCallback?.Invoke(args.Data);
        }

        private void onErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args?.Data != null)
                ErrorDataCallback?.Invoke(args.Data);
        }
    }
}
