using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace hardware_info
{
    class HardwareInfoWindows : HardwareInfo
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetSystemFirmwareTable(
            uint FirmwareTableProviderSignature,
            uint FirmwareTableID,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pFirmwareTableBuffer,
            uint BufferSize);

        public HardwareInfoWindows() : base()
        {
            try { HwInfo = GetHardwareInfo(true); }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
        }

        public override JObject GetHardwareInfo(bool updateStoredInfo = false)
        {
            var hwInfo = new JObject();
            uint rawLenght = 5000;
            byte[] raw = new byte[rawLenght];
            uint sig = 0x52534D42; // RSMB
            uint res = GetSystemFirmwareTable(sig, 0, raw, rawLenght);

            // Отрезаем первые 8 байтов, которые отвечают за инфо о smbios.
            int lenghtSmbiosData = 8;
            byte[] smbiosData = new byte[lenghtSmbiosData];
            for (int i = 0; i < lenghtSmbiosData; i++)
            {
                smbiosData[i] = raw[i];
            }
            JObject smbiosMetadata = new SmbiosMetadataInfo<RawSMBIOSData>(smbiosData).Info;

            byte[] buffer = new byte[rawLenght - lenghtSmbiosData];
            Array.Copy(raw, lenghtSmbiosData, buffer, 0, rawLenght - lenghtSmbiosData);

            if (res == 0 || res > 5000)
            {
                throw new Exception();
            }
            CollectHardwareInfo(ref hwInfo, smbiosMetadata, buffer);
            if (updateStoredInfo)
            {
                HwInfo = hwInfo;
            }
            return hwInfo;
        }
    }
}
