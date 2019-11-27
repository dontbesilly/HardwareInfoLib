using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace hardware_info
{
    /// <summary>
    /// Получение данных по железу.
    /// </summary>
    public interface IHardwareInfo
    {
        /// <summary>
        /// JSON объект с информацией о железе, разбитой на страницы DMI.
        /// </summary>
        JObject HwInfo { get; }

        /// <summary>
        /// Получение данных железа через SMBIOS DMI.
        /// </summary>
        JObject GetHardwareInfo(bool updateStoredInfo = false);

        /// <summary>
        /// Полная информация о smbios как есть из компьютера.
        /// </summary>
        List<byte> SmbiosData { get; }
    }
}