using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemWriteTest
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int nSize,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        static void Main(string[] args)
        {
            int curTickCount = Environment.TickCount;
            
            uint exTickCount = 1;
            byte[] bCurTickCount = BitConverter.GetBytes(exTickCount);
            
            Console.Write("Our value: " + BitConverter.ToUInt32(bCurTickCount, 0) +
                Environment.NewLine +
                "Our value in bytes: ");
            
            foreach (byte b in bCurTickCount)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.Write("\n");

            Process WoW = Process.GetProcessesByName("WoW")[0];

            IntPtr bytesWritten = IntPtr.Zero;
            IntPtr lpBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(bCurTickCount[0]) * 4);
            Marshal.Copy(bCurTickCount, 0, lpBuffer, bCurTickCount.Length);
            WriteProcessMemory(WoW.Handle, (IntPtr)0x00CF0BC8, lpBuffer, bCurTickCount.Length, out bytesWritten);

            Console.WriteLine("We just wrote " + bytesWritten + " bytes to memory!");

            Console.WriteLine("\n\nTime to read LastHardwareAction ...");



            byte[] res = new byte[4];
            IntPtr countOfBytesRead = IntPtr.Zero;
            ReadProcessMemory(WoW.Handle, (IntPtr)0x00CF0BC8, res, 4, out countOfBytesRead);
            Console.WriteLine("We read " + countOfBytesRead + " bytes from memory!");
            Console.Write("The bytes we just read: ");
            foreach (byte b in res)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.Write("\n");
            Console.WriteLine("The bytes as UInt: " + BitConverter.ToUInt32(res, 0));
            Console.Write("\n");
            
        }
    }
}
