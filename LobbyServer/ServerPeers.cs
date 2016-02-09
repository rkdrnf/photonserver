using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using System.Net;

namespace LobbyServer
{
    public class ServerPeers
    {
        public static ServerPeerBase barcarratPeer;

        public static void Setup(ApplicationBase server)
        {
            server.ConnectToServerTcp(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520), "BarcarratServer", null);
        }
    }
}
