using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Helpers
{
    internal class NetworkHelper
    {
        public static void ShareFolder(string folderPath, string shareName)
        {
            if (SharedNameAlreadyExists(shareName))
                return;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            string user = currentUser.Name;

            var psi = new ProcessStartInfo("net", $"share {shareName}={folderPath} /GRANT:{user},FULL")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Failed to share folder: {error}");
                }
            }
        }

        public static void UnshareFolder(string shareName)
        {          
            var psi = new ProcessStartInfo("net", $"share {shareName} /delete")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Failed to unshare folder: {error}");
                }
            }
        }

        public static bool SharedNameAlreadyExists(string shareName)
        {
            var checkPsi = new ProcessStartInfo("net", "share")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(checkPsi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                
                return output.Contains(shareName);
            }
        }
    }
}
