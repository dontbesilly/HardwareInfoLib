using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace hardware_info
{
    abstract class HardwareInfo : IHardwareInfo
    {
        private JArray processorInfo = new JArray();
        private JObject biosInfo = new JObject();
        private JObject systemInfo = new JObject();
        private JObject systemEnclosureInfo = new JObject();

        public JObject HwInfo { get; protected set; } = new JObject();
        public List<byte> SmbiosData { get; private set; }

        public HardwareInfo() { }

        public abstract JObject GetHardwareInfo(bool updateStoredInfo = false);

        protected virtual void CollectHardwareInfo(ref JObject hwInfo, JObject smbiosMetadata, byte[] raw)
        {
            // Сжимаем данные по smbios.
            byte[] compressedData = CompressGzip(raw);
            // Записываем эти данные для последующей отправки если надо.
            SmbiosData = new List<byte>(compressedData);

            // Обходим массив данные 
            CollectSmbiosInfo(raw);

            hwInfo.Add("structureVersion", 1);

            JObject hwData = new JObject();

            // Информация по компьютеру.
            JObject pcInfo = new JObject();
            pcInfo.Add("computer_name", System.Environment.MachineName);
            pcInfo.Add("operating_system", System.Environment.OSVersion.Platform.ToString());
            pcInfo.Add("operating_system_version", System.Environment.OSVersion.Version.ToString());
            pcInfo.Add("operating_system_architecture", System.Environment.Is64BitOperatingSystem ? "x64" : "x32");
            hwData.Add("pc_info", pcInfo);

            // Информация по сетевым интерфейсам. // TODO 
            JObject network = new JObject();
            GetNetInterfaces(out List<string> netInterfaces, out List<NetDevice> netDevices);
            network.Add("net_interfaces", new JArray(netInterfaces));
            network.Add("devices", JArray.FromObject(netDevices));
            hwData.Add("network", network);

            // TODO Информация по HDD 
            // JObject hdd = new JObject();
            // var device = new SystemDrive();
            // hdd.Add("system_drive", JObject.FromObject(device));
            // var devices = new JArray();
            // devices.Add(JObject.FromObject(device));
            // hdd.Add("devices", devices);
            // hwData.Add("hdd", hdd);

            // Информация по SMBIOS.
            JObject smbios = new JObject();
            smbios.Add("metadata", smbiosMetadata);
            smbios.Add("bios_information", biosInfo);
            smbios.Add("system_information", systemInfo);
            smbios.Add("system_enclosure", systemEnclosureInfo);
            smbios.Add("processor_information", processorInfo);
            hwData.Add("smbios", smbios);

            hwInfo.Add("hwData", hwData);
        }

        public class NetDevice
        {
            public string id;
            public string name;
            public string serial_number;
            public string mac;
        }

        public class SystemDrive
        {
            public string id = "";
            public string name = "";
            public string serial_number = "";
        }

        /// <summary>
        /// TODO переделать, вынести в другое место
        /// </summary>
        public static void GetNetInterfaces(out List<string> netInterfaces, out List<NetDevice> netDevices)
        {
            netInterfaces = new List<string>();
            // TOFIX добавляем ещё loopback
            netInterfaces.Add("0.0.0.0");
            netDevices = new List<NetDevice>();
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet || item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                || item.NetworkInterfaceType == NetworkInterfaceType.Loopback) && item.OperationalStatus == OperationalStatus.Up)
                {
                    var netDevice = new NetDevice
                    {
                        id = item.Id,
                        name = item.Name,
                        serial_number = item.Description,
                        mac = item.GetPhysicalAddress().ToString()
                    };
                    netDevices.Add(netDevice);
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            netInterfaces.Add(ip.Address.ToString());
                }
        }

        /// <summary>
        /// Информация по железу из smbios.
        /// </summary>
        private void CollectSmbiosInfo(byte[] raw)
        {
            if (raw == null || raw.Length == 0) { throw new Exception(); }
            int offset = 0;
            byte type = raw[offset];
            while (offset + 4 < raw.Length && type != 127)
            {
                type = raw[offset];
                int length = raw[offset + 1];

                if (offset + length > raw.Length)
                    break;
                byte[] data = new byte[length];
                Array.Copy(raw, offset, data, 0, length);
                offset += length;

                // Данные которые представлены строкой.
                List<string> stringsList = new List<string>();
                if (offset < raw.Length && raw[offset] == 0)
                    offset++;

                while (offset < raw.Length && raw[offset] != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    while (offset < raw.Length && raw[offset] != 0)
                    {
                        sb.Append((char)raw[offset]); offset++;
                    }
                    stringsList.Add(sb.ToString().Trim());
                    offset++;
                }
                offset++;
                switch (type)
                {
                    case 0x00:
                        biosInfo = GetBiosInfo(data, stringsList);
                        break;
                    case 0x01:
                        systemInfo = GetSystemInfo(data, stringsList);
                        break;
                    case 0x03:
                        systemEnclosureInfo = new SystemEnclosureInfo<SystemEnclosureStructure>(data).Info;
                        break;
                    case 0x04:
                        JObject processor = GetProcessorInfo(data, stringsList);
                        processorInfo.Add(processor);
                        break;
                }
            }
        }

        private JObject GetBiosInfo(byte[] data, List<string> stringsList)
        {
            JObject bios = new BiosInfo<BiosStructure>(data, stringsList).Info;
            // Получаем свойство VM.
            JObject vm = new JObject();
            if (data.Length > 19)
            {
                byte characteristics = data[19];
                byte[] arr = new byte[1];
                arr[0] = characteristics;
                var ba = new BitArray(arr);
                bool isVirtualMachine = ba[4];
                vm.Add("virtual_machine", isVirtualMachine);
            }
            else
            {
                vm.Add("virtual_machine", null);
            }
            bios.Add("characteristics", vm);
            return bios;
        }

        private JObject GetSystemInfo(byte[] data, List<string> stringsList)
        {
            JObject system;
            switch (data.Length)
            {
                case 8:
                    system = new SystemInfo<SystemStruct_20>(data, stringsList).Info;
                    break;
                case 25:
                    system = new SystemInfo<SystemStruct_21>(data, stringsList).Info;
                    break;
                case 27:
                    system = new SystemInfo<SystemStruct_24>(data, stringsList).Info;
                    break;
                default:
                    system = new SystemInfo<SystemStruct_24>(data, stringsList).Info;
                    break;
            }
            return system;
        }

        private JObject GetProcessorInfo(byte[] data, List<string> stringsList)
        {
            JObject processor;
            switch (data.Length)
            {
                case 26:
                    processor = new ProcessorInfo<ProcessorStruct_20>(data, stringsList).Info;
                    break;
                case 35:
                    processor = new ProcessorInfo<ProcessorStruct_23>(data, stringsList).Info;
                    break;
                case 40:
                    processor = new ProcessorInfo<ProcessorStruct_25>(data, stringsList).Info;
                    break;
                case 42:
                    processor = new ProcessorInfo<ProcessorStruct_26>(data, stringsList).Info;
                    break;
                case 48:
                    processor = new ProcessorInfo<ProcessorStruct_30>(data, stringsList).Info;
                    break;
                default:
                    processor = new ProcessorInfo<ProcessorStruct_30>(data, stringsList).Info;
                    break;
            }
            return processor;
        }

        /// <summary>
        /// Сжатие данных по smbios с помощью GZip.
        /// </summary>
        private byte[] CompressGzip(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }
    }
}
