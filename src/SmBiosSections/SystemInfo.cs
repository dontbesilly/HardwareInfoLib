using System.Runtime.InteropServices;

namespace HardwareInfo.SmBiosSections;

public class SystemInfo<T> : BaseSmbiosInfo<T> where T : struct
{
    public SystemInfo(byte[] data, List<string> stringsList) :
        base(data, stringsList,
            new List<string> {"manufacturer", "product_name", "version", "serial_number", "sku_number", "family"})
    {
    }
}

[StructLayout(LayoutKind.Explicit, Size = 27, Pack = 1)]
public struct SystemStruct_24
{
    [FieldOffset(0x04)] public byte manufacturer;

    [FieldOffset(0x05)] public byte product_name;

    [FieldOffset(0x06)] public byte version;

    // 2.1

    [FieldOffset(0x07)] public byte serial_number;

    [FieldOffset(0x08)] public Guid uuid;

    // 2.4

    [FieldOffset(0x19)] public byte sku_number;

    [FieldOffset(0x1A)] public byte family;
}

[StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
public struct SystemStruct_20
{
    [FieldOffset(0x04)] public byte manufacturer;

    [FieldOffset(0x05)] public byte product_name;

    [FieldOffset(0x06)] public byte version;
}

[StructLayout(LayoutKind.Explicit, Size = 25, Pack = 1)]
public struct SystemStruct_21
{
    [FieldOffset(0x04)] public byte manufacturer;

    [FieldOffset(0x05)] public byte product_name;

    [FieldOffset(0x06)] public byte version;

    // 2.1

    [FieldOffset(0x07)] public byte serial_number;

    [FieldOffset(0x08)] public Guid uuid;
}