using Newtonsoft.Json.Linq;

namespace HardwareInfo.Info;

/// <summary>
/// Hardware info
/// </summary>
public interface IHardwareInfo
{
    /// <summary>
    /// JSON object  DMI.
    /// </summary>
    JObject HwInfo { get; }

    /// <summary>
    /// Get info SMBIOS DMI.
    /// </summary>
    JObject GetHardwareInfo(bool updateStoredInfo = false);

    /// <summary>
    /// Full smBios data
    /// </summary>
    List<byte> SmBiosData { get; }
}