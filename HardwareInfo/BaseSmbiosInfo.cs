using System;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace hardware_info
{
    public abstract class BaseSmbiosInfo<T> where T : struct
    {
        public JObject Info { get; }

        public BaseSmbiosInfo(byte[] data)
        {
            GCHandle memoryData = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                T param = (T)Marshal.PtrToStructure(memoryData.AddrOfPinnedObject(), typeof(T));
                Info = JObject.FromObject(param);
            }
            catch (Exception ex)
            {
                throw ex;
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
                T param = (T)Marshal.PtrToStructure(memoryData.AddrOfPinnedObject(), typeof(T));
                var info = JObject.FromObject(param);

                // Заполнение строковых значений.
                // Все значения, которые могут быть String берутся из документации.
                for (int i = 0; i < documentationList.Count; i++)
                {
                    try
                    {
                        string name = documentationList[i];
                        string value = stringsList[i];
                        // Если это поле есть в структуре, тогда его меняем.
                        if (info[name] != null)
                            info[name] = value;
                    }
                    catch { continue; }
                }
                Info = info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                memoryData.Free();
            }
        }
    }
}