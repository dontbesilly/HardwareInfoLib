using System;
using hardware_info;

namespace hwinfo
{
    class Program
    {
        static void Main(string[] args)
        {
            IHardwareInfo hardwareInfo;
#if Linux
            hardwareInfo = new HardwareInfoLinux();
#elif Windows
            hardwareInfo = new HardwareInfoWindows();
#endif
            Console.WriteLine(hardwareInfo.HwInfo);
            Console.ReadLine();
        }
    }
}
