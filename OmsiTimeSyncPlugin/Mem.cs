using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public class Mem
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        IntPtr processHandle;
        int processID;

        public Process theProc;

        public Mem()
        {
            processHandle = IntPtr.Zero;
            processID = 0;
            theProc = null;
        }

        public void CloseProcess()
        {
            processHandle = IntPtr.Zero;
            processID = 0;
            theProc = null;
        }

        public bool OpenProcess(int pID)
        {
            processHandle = OpenProcess(PROCESS_WM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, pID);
            processID = pID;
            theProc = Process.GetProcessById(processID);

            if (processHandle != IntPtr.Zero) return true;

            processID = 0;
            theProc = null;

            return false;
        }

        public int ReadByte(int memoryAddress, bool baseAddress = true)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[1];

                    ReadProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);

                    return (int)buffer[0];
                }
                catch { throw new Exception("Invalid value at address"); }
            }

            throw new Exception("Process not attached");
        }

        public byte[] ReadBytes(int memoryAddress, int length, bool baseAddress = true, bool untilNullByte = false)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[length];

                    ReadProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);

                    if (untilNullByte)
                    {
                        for (int i = 0; i < bytesRead; i++)
                        {
                            if (buffer[i] == 0x0 && i > 0)
                            {
                                return buffer.Skip(0).Take(i).ToArray();
                            }
                        }
                    }

                    return buffer;
                }
                catch { throw new Exception("Invalid value at address"); }
            }

            throw new Exception("Process not attached");
        }

        public float ReadFloat(int memoryAddress, bool baseAddress = true)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[4];

                    ReadProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);

                    return BitConverter.ToSingle(buffer, 0);
                }
                catch { throw new Exception("Invalid value at address"); }
            }

            throw new Exception("Process not attached");
        }

        public int ReadInt(int memoryAddress, bool baseAddress = true)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[4];

                    ReadProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);

                    return BitConverter.ToInt32(buffer, 0);
                }
                catch { throw new Exception("Invalid value at address"); }
            }

            throw new Exception("Process not attached");
        }

        public bool WriteMemory(int memoryAddress, string dataType, string newValue, bool baseAddress = true)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;
                    byte[] buffer;

                    switch (dataType)
                    {
                        case "byte":
                            buffer = new byte[1];
                            buffer[0] = Convert.ToByte(newValue);
                            break;

                        case "float":
                            buffer = BitConverter.GetBytes(Convert.ToSingle(newValue));
                            break;

                        case "int":
                            buffer = BitConverter.GetBytes(Convert.ToInt32(newValue));
                            break;

                        case "string":
                            buffer = Encoding.ASCII.GetBytes(newValue);
                            break;

                        default:
                            return false;
                    }

                    return WriteProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);
                }
                catch { }
            }

            return false;
        }

        public bool WriteMemory(int memoryAddress, string dataType, byte[] buffer, bool baseAddress = true)
        {
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    int bytesRead = 0;

                    return WriteProcessMemory((Int32)processHandle, baseAddress ? (Process.GetProcessById(processID).MainModule.BaseAddress.ToInt32() + memoryAddress) : memoryAddress, buffer, buffer.Length, ref bytesRead);
                }
                catch { }
            }

            return false;
        }
    }
}
