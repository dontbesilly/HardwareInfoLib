using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace hardware_info
{
    class HardwareInfoLinux : HardwareInfo
    {
        public HardwareInfoLinux() : base()
        {
            try { HwInfo = GetHardwareInfo(true); }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
        }

        public override JObject GetHardwareInfo(bool updateStoredInfo = false)
        {
            var hwInfo = new JObject();
            byte[] raw = File.ReadAllBytes("/sys/firmware/dmi/tables/DMI");

            // В линукс для данных об smbios есть отдельная таблица.
            byte[] smbiosData = File.ReadAllBytes("/sys/firmware/dmi/tables/smbios_entry_point");
            // Проверка якоря таблицы smbios entry point.
            bool is32Smbios = smbiosData[0] == 95 && smbiosData[1] == 83 && smbiosData[2] == 77 && smbiosData[3] == 95;
            bool is64Smbios = smbiosData[0] == 95 && smbiosData[1] == 83 && smbiosData[2] == 77 && smbiosData[3] == 51 && smbiosData[4] == 95;

            JObject smbiosMetadata;
            if (is64Smbios)
                smbiosMetadata = new SmbiosMetadataInfo<EntryPointStructure64>(smbiosData).Info;
            else if (is32Smbios)
                smbiosMetadata = new SmbiosMetadataInfo<EntryPointStructure32>(smbiosData).Info;
            else
                smbiosMetadata = new JObject(new { error = "incorrect anchor" });

            CollectHardwareInfo(ref hwInfo, smbiosMetadata, raw);
            if (updateStoredInfo)
            {
                HwInfo = hwInfo;
            }
            return hwInfo;
        }
    }
}
