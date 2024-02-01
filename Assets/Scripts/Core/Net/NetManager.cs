// 连接网络并开启新线程监听
using System.Collections.Generic;
using System.Threading;
using TGClient;
using Unity.VisualScripting;

namespace XH
{
    class NetManager : CSharpSingleton<NetManager>
    {
        public static readonly Net peerClient = new Net();
        private static readonly Thread netThread = new Thread(NetWork);
        private readonly string ip = "127.0.0.1";
        private readonly int port = 4999;


        public NetManager() 
        {
            EventManager.Instance.AddListener<EventType, Dictionary<short, object>>(EventType.LOGIC_ServerToClient,  ClientToInternal);
        }

        private static void NetWork()
        {
            while(true)
                peerClient.Service();
        }

        public void StartNetThread()
        {
            peerClient.Connect(ip, port);
            netThread.IsBackground = true;
            netThread.Start();
        }

        private  void ClientToInternal(EventType eventType, Dictionary<short, object> dict)
        {
            EventManager.Instance.Broadcast(eventType, dict, true);
        }

        ~NetManager()
        {
            EventManager.Instance.RemoveListener<EventType, Dictionary<short, object>>(EventType.LOGIC_ServerToClient, ClientToInternal);
        }
    }
}
