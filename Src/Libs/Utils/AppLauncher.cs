using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MMK.Utils
{
    /// <summary>
    /// Wrap over Process class that starts single instance of application
    /// Must be implemented path
    /// Instance static method
    /// </summary>
    public abstract class SingleAppLauncher
    {
        public abstract string Path
        {
            get;
        }

        private Process process;

        private string ValidateArguments(IEnumerable<string> args)
        {
            if (args == null) return null;

            return args
                .Select(ValidateArgument)
                .Where(validatedArg => !string.IsNullOrWhiteSpace(validatedArg))
                .Aggregate(
                    string.Empty,
                    (current, validatedArg) => string.Format("{0} \"{1}\"",current, validatedArg)
                );
        }

        protected virtual string ValidateArgument(string arg)
        {
            return arg;
        }

        public void Launch(IEnumerable<string> args = null)
        {
            if (IsRunning)
                SwitchToThisWindow(process.MainWindowHandle);
            else
            {
                var args_ = ValidateArguments(args);
                process = Process.Start(Path, args_);
            }
        }

        private bool IsRunning
        {
            get { return !(process == null || process.HasExited); }
        }

        [DllImport("user32.dll")]
        private extern static void SwitchToThisWindow(IntPtr handle, bool isAlt = false);
    }
}
