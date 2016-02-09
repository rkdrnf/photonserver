using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace GameServer
{
    public class GameServer : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (initRequest.LocalPort == 4520) //S2S Connection
            {
                return new LobbyPeer(initRequest);
            }

            return GamePeer(initRequest);
        }


        protected override void Setup()
        {
            GameServerManager.Instance.Setup();
        }

        protected override void TearDown()
        {
            GameServerManager.Instance.TearDown();
        }

    }
}
