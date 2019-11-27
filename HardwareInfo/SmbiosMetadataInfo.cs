using System.Runtime.InteropServices;

namespace hardware_info
{
    public class SmbiosMetadataInfo<T> : BaseSmbiosInfo<T> where T : struct
    {
        public SmbiosMetadataInfo(byte[] data) : base(data) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 30, Pack = 1)]
    public struct EntryPointStructure32
    {
        [FieldOffset(0x06)]
        public byte major;

        [FieldOffset(0x07)]
        public byte minor;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
    public struct EntryPointStructure64
    {
        [FieldOffset(0x07)]
        public byte major;

        [FieldOffset(0x08)]
        public byte minor;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
    public struct RawSMBIOSData
    {
        [FieldOffset(0x01)]
        public byte major;

        [FieldOffset(0x02)]
        public byte minor;
    }
}