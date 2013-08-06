//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace SupportLibrary
{
    public static class WindowsApi
    {
        private const string KERNEL32 = "kernel32.dll";

        //DWORD WINAPI GetCurrentThreadId(void);
        //http://msdn.microsoft.com/en-us/library/windows/desktop/ms683183(v=vs.85).aspx
        [DllImport(KERNEL32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern uint GetCurrentThreadId();
    }
}
