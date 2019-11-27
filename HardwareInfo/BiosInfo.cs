using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace hardware_info
{
    public class BiosInfo<T> : BaseSmbiosInfo<T> where T : struct
    {
        public BiosInfo(byte[] data, List<string> stringsList) :
        base(data, stringsList, new List<string> { "vendor", "bios_version" })
        { }

    }

    [StructLayout(LayoutKind.Explicit, Size = 24, Pack = 1)]
    public struct BiosStructure
    {
        [FieldOffset(0x04)]
        public byte vendor;

        [FieldOffset(0x05)]
        public byte bios_version;
    }
}