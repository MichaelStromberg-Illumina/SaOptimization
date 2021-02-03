using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace NirvanaCommon
{
    public static class FileCacheBlaster
    {
        private const uint GENERIC_READ           = 0x80000000;
        private const uint OPEN_EXISTING          = 3;
        private const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        
        public static void Blast(string saPath, string indexPath)
        {
            Blast(saPath);
            Blast(indexPath);
        }
        
        public static void Blast(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            
            // .NET 5 makes this even better with OperatingSystem.IsWindows(), etc.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) BlastWindows(path);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) BlastLinux();
        }

        private static void BlastLinux()
        {
            const string dropCachePath = "/proc/sys/vm/drop_caches";

            // requires elevated permissions
            try
            {
                using (var fileStream = new FileStream(dropCachePath, FileMode.Open, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine("3");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Unable to clear the file cache: {e.Message}");
                Environment.Exit(1);
            }
        }

        private static void BlastWindows(string path)
        {
            SafeHandle handle = CreateFile(path, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_NO_BUFFERING,
                IntPtr.Zero);
            handle.Dispose();
        }

        private sealed class SafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeHandle() : base(ownsHandle: true)
            {
            }

            protected override bool ReleaseHandle() => CloseHandle(handle);
        }
        
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeHandle CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}