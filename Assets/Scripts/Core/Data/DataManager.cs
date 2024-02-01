using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XH
{
    class DataManager : CSharpSingleton<DataManager>
    {
        private Dictionary<string, ValueType> dataDict = new Dictionary<string, ValueType>();

        public DataManager()
        {

        }

        ~DataManager()
        {

            dataDict.Clear();
        }

        public void AddData(string dataName, ValueType data)
        {
            XHLogger.XH_ASSERT(!dataDict.ContainsKey(dataName), string.Format("DataManager已存在数据{0}", dataName));
            dataDict.Add(dataName, data);
        }

        public void RefreshData(string dataName, ValueType data, EventType eventType)
        {
            XHLogger.XH_ASSERT(dataDict.ContainsKey(dataName), string.Format("DataManager没有数据{0}", dataName));
            dataDict[dataName] = data;
            EventManager.Instance.Broadcast(eventType, data);
        }
    }
}
