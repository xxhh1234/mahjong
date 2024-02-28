// 连接网络并开启新线程监听
using System.Collections.Generic;
using System.Threading;

namespace XH
{
    class NetManager : CSharpSingleton<NetManager>
    {
        public static readonly Net peerClient = new Net();
        private static readonly Thread netThread = new Thread(NetWork);
        private readonly string ip = "127.0.0.1";
        private readonly int port = 4999;

        public void Init()
        {
            EventManager.Instance.AddListener<short, Dictionary<short, object>, bool>
                (ClientToServer._ClientToServer, SendMessageToInternal);
            EventManager.Instance.AddListener<short, Dictionary<short, object>, bool>
                (ServerToClient._ServerToClient, SendMessageToInternal);
            peerClient.Connect(ip, port);
            netThread.IsBackground = true;
            netThread.Start();
        }
        public void UnInit()
        {
            netThread.Abort();
            peerClient.DisConnect();
            EventManager.Instance.RemoveListener<short, Dictionary<short, object>, bool>
                (ClientToServer._ClientToServer, SendMessageToInternal);
            EventManager.Instance.RemoveListener<short, Dictionary<short, object>, bool>
                (ServerToClient._ServerToClient, SendMessageToInternal);
        }

        private static void NetWork()
        {
            while (true)
                peerClient.Service();
        }
        private  void SendMessageToInternal(short eventCode, Dictionary<short, object> dict, bool isRequest)
        {
            EventManager.Instance.Broadcast(eventCode, dict, isRequest);
        }
    }
}
