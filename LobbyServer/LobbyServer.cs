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
    public class LobbyServer : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new LobbyClientPeer(initRequest);
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            ServerPeers.BaccaratPeer = new LobbyOutPeer(initResponse.Protocol, initResponse.PhotonPeer);
            return ServerPeers.BaccaratPeer;
        }

        protected override void Setup()
        {
            ServerPeers.Setup(this);
        }

        protected override void TearDown()
        {
            throw new NotImplementedException();
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            // add some custom error handling here
        }
    }
}
