using System.Runtime.InteropServices;

namespace HardwareInfo.SmBiosSections;

public class ProcessorInfo<T> : BaseSmbiosInfo<T> where T : struct
{
    public ProcessorInfo(byte[] data, List<string> stringsList) :
        base(data, stringsList,
            new List<string>
            {
                "socket_designation", "processor_manufacturer", "processor_version", "serial_number", "asset_tag",
                "part_number"
            })
    {
    }
}

[StructLayout(LayoutKind.Explicit, Size = 48, Pack = 1)]
public struct ProcessorStruct_30
{
    [FieldOffset(0x05)] public byte processor_type;

    [FieldOffset(0x06)] public byte processor_family;

    [FieldOffset(0x07)] public byte processor_manufacturer;

    [FieldOffset(0x08)] public Guid processor_id;

    [FieldOffset(0x10)] public byte processor_version;

    // 2.3

    [FieldOffset(0x20)] public byte serial_number;

    [FieldOffset(0x22)] public byte part_number;

    // 2.5

    [FieldOffset(0x23)] public byte core_count;

    [FieldOffset(0x24)] public byte core_enabled;

    [FieldOffset(0x25)] public byte thread_count;

    // 2.6

    [FieldOffset(0x28)] public short processor_family_2;

    // 3.0

    [FieldOffset(0x2A)] public short core_count_2;

    [FieldOffset(0x2C)] public short core_enabled_2;

    [FieldOffset(0x2E)] public short thread_count_2;
}

[StructLayout(LayoutKind.Explicit, Size = 26, Pack = 1)]
public struct ProcessorStruct_20
{
    [FieldOffset(0x05)] public byte processor_type;

    [FieldOffset(0x06)] public byte processor_family;

    [FieldOffset(0x07)] public byte processor_manufacturer;

    [FieldOffset(0x08)] public Guid processor_id;

    [FieldOffset(0x10)] public byte processor_version;
}

[StructLayout(LayoutKind.Explicit, Size = 35, Pack = 1)]
public struct ProcessorStruct_23
{
    [FieldOffset(0x05)] public byte processor_type;

    [FieldOffset(0x06)] public byte processor_family;

    [FieldOffset(0x07)] public byte processor_manufacturer;

    [FieldOffset(0x08)] public Guid processor_id;

    [FieldOffset(0x10)] public byte processor_version;

    // 2.3

    [FieldOffset(0x20)] public byte serial_number;

    [FieldOffset(0x22)] public byte part_number;
}

[StructLayout(LayoutKind.Explicit, Size = 40, Pack = 1)]
public struct ProcessorStruct_25
{
    [FieldOffset(0x05)] public byte processor_type;

    [FieldOffset(0x06)] public byte processor_family;

    [FieldOffset(0x07)] public byte processor_manufacturer;

    [FieldOffset(0x08)] public Guid processor_id;

    [FieldOffset(0x10)] public byte processor_version;

    // 2.3

    [FieldOffset(0x20)] public byte serial_number;

    [FieldOffset(0x22)] public byte part_number;

    // 2.5

    [FieldOffset(0x23)] public byte core_count;

    [FieldOffset(0x24)] public byte core_enabled;

    [FieldOffset(0x25)] public byte thread_count;
}

[StructLayout(LayoutKind.Explicit, Size = 42, Pack = 1)]
public struct ProcessorStruct_26
{
    [FieldOffset(0x05)] public byte processor_type;

    [FieldOffset(0x06)] public byte processor_family;

    [FieldOffset(0x07)] public byte processor_manufacturer;

    [FieldOffset(0x08)] public Guid processor_id;

    [FieldOffset(0x10)] public byte processor_version;

    // 2.3

    [FieldOffset(0x20)] public byte serial_number;

    [FieldOffset(0x22)] public byte part_number;

    // 2.5

    [FieldOffset(0x23)] public byte core_count;

    [FieldOffset(0x24)] public byte core_enabled;

    [FieldOffset(0x25)] public byte thread_count;

    // 2.6

    [FieldOffset(0x28)] public short processor_family_2;
}