using HardwareInfo.SmBiosSections;
using Newtonsoft.Json.Linq;

namespace HardwareInfo.Info;

public class HardwareInfoLinux : HardwareInfo
{
    public HardwareInfoLinux() : base()
    {
        HwInfo = GetHardwareInfo(true);
    }

    public sealed override JObject GetHardwareInfo(bool updateStoredInfo = false)
    {
        var hwInfo = new JObject();
        byte[] raw = File.ReadAllBytes("/sys/firmware/dmi/tables/DMI");

        byte[] smBiosData = File.ReadAllBytes("/sys/firmware/dmi/tables/smbios_entry_point");

        var is32SmBios = smBiosData[0] == 95 && smBiosData[1] == 83 && smBiosData[2] == 77 && smBiosData[3] == 95;
        var is64SmBios = smBiosData[0] == 95 && smBiosData[1] == 83 && smBiosData[2] == 77 &&
                         smBiosData[3] == 51 && smBiosData[4] == 95;

        JObject smBiosMetadata;
        if (is64SmBios)
            smBiosMetadata = new SmbiosMetadataInfo<EntryPointStructure64>(smBiosData).Info;
        else if (is32SmBios)
            smBiosMetadata = new SmbiosMetadataInfo<EntryPointStructure32>(smBiosData).Info;
        else
            smBiosMetadata = new JObject(new {error = "incorrect anchor"});

        CollectHardwareInfo(ref hwInfo, smBiosMetadata, raw);
        if (updateStoredInfo)
        {
            HwInfo = hwInfo;
        }

        return hwInfo;
    }
}