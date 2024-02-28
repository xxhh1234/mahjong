using System.Collections.Generic;
using TGClient;
using System;
using System.Threading.Tasks;

namespace XH
{
    public class Net : PeerBase
    {
        public override void OnOperationResponse(short opreationCode, ReceiveResponse response)
        {
            if(response.returnCode != 0)
                Logger.XH_LOG((string)response.parameters[ParameterCode.error]);
            else
                EventManager.Instance.Broadcast(ClientToServer._ClientToServer, opreationCode, response.parameters, true, false);
            
        }
        public override void OnEvent(short eventCode, Dictionary<short, object> dict)
        {
            EventManager.Instance.Broadcast(ServerToClient._ServerToClient, eventCode, dict, false, false);
        }
        
        public override void OnConnected(string message)
        {
            Logger.XH_LOG(message);
        }
        public override void OnDisConnect(Exception connectException)
        {
            Logger.XH_LOG(connectException.ToString());
        }
        public override void OnException(Exception exception)
        {
            Logger.XH_LOG(exception.ToString());
        }

        public async void SendRequestAsync(short opCode, Dictionary<short, object> opDict, 
            Action<Dictionary<short, object>> action=null)
        {
            await Task.Run(() =>
            {
                if (action != null)
                    EventManager.Instance.AddListener(opCode, (Dictionary<short, object> respDict) =>
                    {
                        ThreadManager.ExecuteUpdate(() => { action(respDict);});
                    });
                SendRequest(opCode, opDict);
            });           
        }
        public void SendRequestSync(short opCode, Dictionary<short, object> opDict,
            Action<Dictionary<short, object>> action = null)
        {
            if (action != null)
                EventManager.Instance.AddListener(opCode, (Dictionary<short, object> respDict) =>
                {
                    ThreadManager.ExecuteUpdate(() => { action(respDict); });
                });
            SendRequest(opCode, opDict);
        }
    }
}