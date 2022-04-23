using System.Runtime.InteropServices;

namespace HardwareInfo.SmBiosSections;

public class SystemEnclosureInfo<T> : BaseSmbiosInfo<T> where T : struct
{
    public SystemEnclosureInfo(byte[] data) : base(data)
    {
    }
}

[StructLayout(LayoutKind.Explicit, Size = 21, Pack = 1)]
public struct SystemEnclosureStructure
{
    [FieldOffset(0x05)] public byte type;
}