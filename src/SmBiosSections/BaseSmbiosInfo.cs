using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace HardwareInfo.SmBiosSections;

public abstract class BaseSmbiosInfo<T> where T : struct
{
    public JObject Info { get; }

    public BaseSmbiosInfo(byte[] data)
    {
        GCHandle memoryData = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            T param = (T) Marshal.PtrToStructure(memoryData.AddrOfPinnedObject(), typeof(T));
            Info = JObject.FromObject(param);
        }
        finally
        {
            memoryData.Free();
        }
    }

    public BaseSmbiosInfo(byte[] data, List<string> stringsList, List<string> documentationList)
    {
        GCHandle memoryData = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            T param = (T) Marshal.PtrToStructure(memoryData.AddrOfPinnedObject(), typeof(T));
            var info = JObject.FromObject(param);

            // All values that can be String are taken from the documentation
            for (var i = 0; i < documentationList.Count; i++)
            {
                try
                {
                    var name = documentationList[i];
                    var value = stringsList[i];

                    if (info[name] != null)
                        info[name] = value;
                }
                catch
                {
                    // ignored
                }
            }

            Info = info;
        }
        finally
        {
            memoryData.Free();
        }
    }
}