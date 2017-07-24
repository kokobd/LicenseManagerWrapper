using System;
using System.Diagnostics;

namespace JiaHe.LicenseManager
{
    class CmdUtils
    {
        public static String RunCommand(String exec, String args, ref int exitCode)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = exec;
            cmd.StartInfo.Arguments = args;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.WaitForExit();
            exitCode = cmd.ExitCode;
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}
