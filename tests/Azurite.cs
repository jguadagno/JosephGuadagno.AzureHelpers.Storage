using System;
using System.Diagnostics;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    /// <summary>
    /// Methods to start the Azurite client
    /// </summary>
    /// <remarks>This is a work in process. I want to try and trigger getting Azurite started at the beginning or executing the tests.</remarks>
    public static class Azurite
    {
        public static Process Start(string blobHost = "127.0.0.1", string blobPort = "10000", 
            string queueHost = "127.0.0.1", string queuePort = "10001", 
            string workFolder = null, string debugLogFile = null)
        {
            var commandLine =
                $"--blobHost {blobHost} --blobPort {blobPort} --queueHost {queueHost} --queuePort {queuePort}";

            if (!string.IsNullOrEmpty(workFolder))
            {
                commandLine += $" --location {workFolder}";
            }

            if (!string.IsNullOrEmpty(debugLogFile))
            {
                commandLine += $" --debug {debugLogFile}";
            }

            return StartWithCustomArguments(commandLine);
        }
        private static Process StartWithCustomArguments(string args)
        {
            Process process = null;
            var startInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = args,
                FileName = "Azurite"
            };
            
            try
            {
                process = new Process { StartInfo = startInfo };

                process.Start();
            }
            catch (Exception)
            {
                if (process == null)
                {
                    return null;
                }

                process.CloseMainWindow();
                process.Dispose();
                return null;
            }

            return process;
        }
    }
}