using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            // run wow.exe in a new process
            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
            CreateProcess(
                PATH_TO_GAME,
                null,
                ref pSec,
                ref tSec,
                false,
                0x04000000, // CREATE_DEFAULT_ERROR_MODE
                IntPtr.Zero,
                null,
                ref startupInfo,
                out PROCESS_INFORMATION processInfo);

            // get a handle to the BloogsQuest process
            var processHandle = Process.GetProcessById(processInfo.dwProcessId).Handle;

            // resolve the file path to Loader.dll relative to our current working directory
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var loaderPath = $"{currentFolder}\\Loader.dll";

            // allocate enough memory to hold the full file path to Loader.dll within the BloogsQuest process
            var loaderPathPtr = VirtualAllocEx(
                processHandle,
                (IntPtr)0,
                (uint) loaderPath.Length,
                AllocationType.Commit, // MEM_COMMIT
                MemoryProtection.ExecuteReadWrite // PAGE_EXECUTE_READWRITE
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
            VirtualFreeEx(processHandle, loaderPathPtr, 0, AllocationType.Release /* MEM_RELEASE */);
        }
    }
}
