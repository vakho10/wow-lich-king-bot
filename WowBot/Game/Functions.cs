using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WowBot.Game
{
    class Functions
    {
        const int YELL_FUNCTION_OFFSET = 0x114B0;

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void YellDelegate();

        YellDelegate YellFunction;

        public Functions()
        {
            YellFunction = Marshal.GetDelegateForFunctionPointer(
                Process.GetCurrentProcess().MainModule.BaseAddress + YELL_FUNCTION_OFFSET,
                typeof(YellDelegate)) as YellDelegate;
        }

        internal void Yell() => YellFunction();
    }
}
