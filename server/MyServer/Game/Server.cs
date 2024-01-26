using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGServer;

namespace MyServer
{
    internal class Server : TGServerBase
    {
        public Server(string address, int port)
            :base(address, port)
        {

        }

        public override PeerBase GetPeer()
        {
            Peer peer = new Peer();
            return peer;
        }

        public override void OnServerException(Exception serverException)
        {
            Console.WriteLine("OnServerException");
        }

        public override void OnServerStart()
        {
            Console.WriteLine("OnServerStart");
        }
    }
}
