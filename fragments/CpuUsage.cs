//Sample provided by Fabio Galuppo
//June 2014

//Based on http://www.codeproject.com/Articles/10258/How-to-get-CPU-usage-of-processes-and-threads 
//Refactored and Reorganized for General Purpose Usage

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using ct = System.Runtime.InteropServices.ComTypes;

namespace WindowsProcessUtil
{
    internal static class WinAPI
    {
        //http://www.pinvoke.net/default.aspx/kernel32/CreateToolhelp32Snapshot.html
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        //http://www.pinvoke.net/default.aspx/kernel32/Process32First.html
        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        //http://www.pinvoke.net/default.aspx/kernel32/Process32Next.html
        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        //http://www.pinvoke.net/default.aspx/kernel32/CloseHandle.html
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        //http://www.pinvoke.net/default.aspx/kernel32/OpenProcess.html
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

        //http://www.pinvoke.net/default.aspx/kernel32/GetProcessTimes.html
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessTimes(IntPtr hProcess, out ct.FILETIME lpCreationTime, out ct.FILETIME lpExitTime, out ct.FILETIME lpKernelTime, out ct.FILETIME lpUserTime);

        public static readonly int PROCESSENTRY32_SIZE = Marshal.SizeOf(typeof(WinAPI.PROCESSENTRY32));
        public const uint SNAPSHOT_FLAGS_PROCESS = 0x00000002;
        public const uint PROCESS_ACCESS_FLAGS_ALL = 0x1F0FFF;

        public static readonly IntPtr PROCESS_LIST_ERROR = new IntPtr(-1);
        public static readonly IntPtr PROCESS_HANDLE_ERROR = new IntPtr(-1);

        //http://www.pinvoke.net/default.aspx/Structures/SYSTEMTIME.html
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            [MarshalAs(UnmanagedType.U2)] public short Year;
            [MarshalAs(UnmanagedType.U2)] public short Month;
            [MarshalAs(UnmanagedType.U2)] public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)] public short Day;
            [MarshalAs(UnmanagedType.U2)] public short Hour;
            [MarshalAs(UnmanagedType.U2)] public short Minute;
            [MarshalAs(UnmanagedType.U2)] public short Second;
            [MarshalAs(UnmanagedType.U2)] public short Milliseconds;
        }

        //http://www.pinvoke.net/default.aspx/kernel32/PROCESSENTRY32.html
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };
    }

    internal struct ProcessTimes
    {
        public DateTime CreationTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        public DateTime KernelTime { get; private set; }
        public DateTime UserTime { get; private set; }

        private ct.FILETIME RawCreationTime, RawExitTime, RawKernelTime, RawUserTime;

        private void Update()
        {
            CreationTime = FileTimeToDateTime(RawCreationTime);
            ExitTime = FileTimeToDateTime(RawExitTime);
            KernelTime = FileTimeToDateTime(RawKernelTime);
            UserTime = FileTimeToDateTime(RawUserTime);
        }

        private static DateTime FileTimeToDateTime(ct.FILETIME fileTime)
        {
            try
            {
                if (fileTime.dwLowDateTime < 0) fileTime.dwLowDateTime = 0;
                if (fileTime.dwHighDateTime < 0) fileTime.dwHighDateTime = 0;
                return DateTime.FromFileTimeUtc((((long)fileTime.dwHighDateTime) << 32) + fileTime.dwLowDateTime);
            }
            catch (Exception) 
            {                
            }

            return new DateTime(); 
        }

        [ThreadStatic]
        public static ProcessTimes processTimes = new ProcessTimes();

        public static ProcessTimes GetFromProcess(IntPtr hProcess)
        {
            WinAPI.GetProcessTimes(hProcess, out processTimes.RawCreationTime, out processTimes.RawExitTime, out processTimes.RawKernelTime, out processTimes.RawUserTime);
            processTimes.Update();
            return processTimes;
        }
    };

    internal sealed class ProcessEnumerable : IEnumerable, IEnumerable<WinAPI.PROCESSENTRY32>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<WinAPI.PROCESSENTRY32> GetEnumerator()
        {
            return new ProcessEnumerator();
        }

        private sealed class ProcessEnumerator : IEnumerator, IEnumerator<WinAPI.PROCESSENTRY32>
        {
            WinAPI.PROCESSENTRY32 ProcessInfo = new WinAPI.PROCESSENTRY32();
            IntPtr ProcessList = IntPtr.Zero;            
            bool IsNext;
            
            public ProcessEnumerator()
            {
                Reset();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (IsNext)
                    return WinAPI.Process32Next(ProcessList, ref ProcessInfo);
                
                IsNext = true;
                if (ProcessList != WinAPI.PROCESS_LIST_ERROR)
                {
                    ProcessInfo.dwSize = (uint)WinAPI.PROCESSENTRY32_SIZE;
                    return WinAPI.Process32First(ProcessList, ref ProcessInfo);
                }
                return false;
            }

            public void Reset()
            {
                Dispose();
                IsNext = false;
                ProcessList = WinAPI.CreateToolhelp32Snapshot(WinAPI.SNAPSHOT_FLAGS_PROCESS, 0);
            }

            public WinAPI.PROCESSENTRY32 Current
            {
                get { return ProcessInfo; }
            }

            public void Dispose()
            {
                if (ProcessList != IntPtr.Zero)
                    WinAPI.CloseHandle(ProcessList);
            }
        }
    }

    public sealed class ProcessInfo
    {
        private struct PreviousTime
        {
            public long UserTime;
            public long KernelTime;
            public DateTime Now;
        }

        public readonly string Name;
        public readonly uint Id;
        public double CpuUsagePercent { get; private set; }
        private PreviousTime Previous;

        //Process : % Processor Time (check Performance Monitor/Counters)
        public string CpuUsageString
        {
            get { return String.Format("{0:0.00}%", CpuUsagePercent); }
        }

        internal ProcessInfo(uint processId, string processName, long previousUserTime, long previousKernelTime)
        {
            Id = processId;
            Name = processName;
            Previous = new PreviousTime()
            {
                UserTime = previousUserTime,
                KernelTime = previousKernelTime,
                Now = DateTime.Now
            };
        }

        internal double RefreshCpuUsage(long newUserTime, long newKernelTime)
        {
            long userTime = newUserTime - Previous.UserTime;
            long kernelTime = newKernelTime - Previous.KernelTime;

            long delta = DateTime.Now.Ticks - Previous.Now.Ticks;
            double rawUsage = ((userTime + kernelTime) * 100.0) / delta;

            Previous.UserTime = newUserTime;
            Previous.KernelTime = newKernelTime;
            Previous.Now = DateTime.Now;

            CpuUsagePercent = rawUsage;
            return rawUsage;
        }
    }

    /// <summary>
    /// Acquiring % Processor Time from Process 
    /// </summary>
    /// <example>
    ///     var cpuUsage = new WindowsProcessUtil.CpuUsage();
    ///     while (true)
    ///     {
    ///         Console.Clear();
    ///         
    ///         foreach (var p in cpuUsage.GetNext()
    ///                             .Where(p => p.CpuUsagePercent > 0)
    ///                             .OrderByDescending(p => p.CpuUsagePercent)
    ///                             .ThenBy(p => p.Name))
    ///         {
    ///             var previousColor = Console.BackgroundColor;
    ///             if (p.CpuUsagePercent > 50.0)
    ///                 Console.BackgroundColor = ConsoleColor.DarkRed;
    ///             else if (25.0 <= p.CpuUsagePercent && p.CpuUsagePercent <= 50.0)
    ///                 Console.BackgroundColor = ConsoleColor.DarkYellow;
    ///
    ///             Console.WriteLine(p.Name + ":" + p.Id + " " + p.CpuUsageString);
    ///
    ///             Console.BackgroundColor = previousColor;
    ///         }
    ///         
    ///         Thread.Sleep(1000); //Refresh Time/Next Update
    ///     }
    /// </example>
    public sealed class CpuUsage
    {
        private List<ProcessInfo> ProcessInfoList = new List<ProcessInfo>();
        private List<uint> ProcessIdList = new List<uint>();

        public IEnumerable<ProcessInfo> GetNext()
        {
            IntPtr procHandle = WinAPI.PROCESS_HANDLE_ERROR;
            
            ProcessIdList.Clear();

            foreach(var pi in new ProcessEnumerable())
            {
                uint pid = pi.th32ProcessID;

                if (pid == 0)
                    continue;

                try
                {
                    procHandle = WinAPI.OpenProcess(WinAPI.PROCESS_ACCESS_FLAGS_ALL, false, pid);

                    ProcessTimes processTimes = ProcessTimes.GetFromProcess(procHandle);
                    long userTime = processTimes.UserTime.Ticks;
                    long kernelTime = processTimes.KernelTime.Ticks;

                    ProcessIdList.Add(pid);

                    ProcessInfo pd = ProcessInfoList.Where(p => p.Id == pid).FirstOrDefault();
                    if (pd != default(ProcessInfo))
                    {
                        pd.RefreshCpuUsage(userTime, kernelTime);
                    }
                    else
                    {
                        ProcessInfoList.Add(new ProcessInfo(pid, pi.szExeFile, userTime, kernelTime)); 
                    }
                }
                finally
                {
                    if (procHandle != WinAPI.PROCESS_HANDLE_ERROR)
                        WinAPI.CloseHandle(procHandle);
                }
            }

            //foreach(var x in ProcessInfoList.Where(pd => !ProcessIdList.Contains(pd.Id)).ToArray())
            //    ProcessInfoList.Remove(x);
            //or
            ProcessInfoList.RemoveAll(pd => !ProcessIdList.Contains(pd.Id));

            return ProcessInfoList;
        }
    }
}