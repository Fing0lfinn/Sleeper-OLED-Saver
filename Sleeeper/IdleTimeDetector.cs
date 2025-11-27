using System.Runtime.InteropServices;

namespace Sleeper
{
    public static class IdleTimeDetector
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll")]
        private static extern ulong GetTickCount64();

        public static double GetIdleTimeSeconds()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            if (!GetLastInputInfo(ref lastInputInfo))
            {
                return 0;
            }

            ulong currentTimeMs = GetTickCount64();
            uint lastInputTimeMs = lastInputInfo.dwTime;
            double idleTimeMs = currentTimeMs - lastInputTimeMs;
            return idleTimeMs / 1000.0;
        }
    }
}