using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using static Bootstrapper.WinImports;

namespace Bootstrapper
{
    class Program
    {
        // TODO show browse input!
        const string PATH_TO_GAME = @"C:\Users\v.laluashvili\source\repos\BloogsQuest\x64\Release\BloogsQuest.exe";

        static void Main(string[] args)
        {
            var startupInfo = new STARTUPINFO();

            // run BloogsQuest.exe in a new process
            CreateProcess(
                PATH_TO_GAME,
                null,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x04000000, // CREATE_DEFAULT_ERROR_MODE
                IntPtr.Zero,
                null,
                ref startupInfo,
                out PROCESS_INFORMATION processInfo);

            // get a handle to the BloogsQuest process
            var processHandle = Process.GetProcessById((int)processInfo.dwProcessId).Handle;

            // resolve the file path to Loader.dll relative to our current working directory
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var loaderPath = $"{currentFolder}\\Loader.dll";

            // allocate enough memory to hold the full file path to Loader.dll within the BloogsQuest process
            var loaderPathPtr = VirtualAllocEx(
                processHandle,
                (IntPtr)0,
                loaderPath.Length,
                AllocationType.Commit, // MEM_COMMIT
                MemoryProtection.ReadWrite // PAGE_EXECUTE_READWRITE
            );

            // write the file path to Loader.dll to the BloogsQuest process's memory
            var bytes = Encoding.Unicode.GetBytes(loaderPath);
            var bytesWritten = 0; // throw away
            WriteProcessMemory(processHandle, loaderPathPtr, bytes, bytes.Length, ref bytesWritten);

            // search current process's for the memory address of the LoadLibraryW function within the kernel32.dll module
            var loaderDllPointer = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");

            // create a new thread with the execution starting at the LoadLibraryW function, 
            // with the path to our Loader.dll passed as a parameter
            CreateRemoteThread(processHandle, (IntPtr)null, 0, loaderDllPointer, loaderPathPtr, 0, (IntPtr)null);

            // free the memory that was allocated by VirtualAllocEx
            VirtualFreeEx(processHandle, loaderPathPtr, 0, AllocationType.Release);
        }
    }
}
