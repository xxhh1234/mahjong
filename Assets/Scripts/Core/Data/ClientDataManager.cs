using System;
using System.Collections.Generic;

namespace XH
{
    class ClientDataManager : CSharpSingleton<ClientDataManager>
    {
        private Dictionary<string, ValueType> dataDict = new Dictionary<string, ValueType>();

        public void AddData(string dataName, ValueType data)
        {
            Logger.XH_ASSERT(!dataDict.ContainsKey(dataName), string.Format("DataManager已存在数据{0}", dataName));
            dataDict.Add(dataName, data);
        }
        public ValueType GetData(string dataName)
        {
            Logger.XH_ASSERT(dataDict.ContainsKey(dataName), string.Format("DataManager没有数据{0}", dataName));
            return dataDict[dataName];
        }
        public void RefreshData(string dataName, ValueType data, short dataEventCode=0)
        {
            Logger.XH_ASSERT(dataDict.ContainsKey(dataName), string.Format("DataManager没有数据{0}", dataName));
            dataDict[dataName] = data;
            if(dataEventCode != 0)
                EventManager.Instance.Broadcast(dataEventCode, data);
        }
    }
}
