using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Photon.SocketServer;
using Baccarats;

    public class BaccaratServer : GameServer
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (initRequest.LocalPort == 4520) //S2S Connection
            {
                return new LobbyPeer(initRequest);
            }

            return new BaccaratPeer(initRequest);
        }

        protected override void Setup()
        {
            base.Setup();
        }

        protected override void TearDown()
        {
            base.TearDown();
        }

        protected override void SetupServerManager()
        {
            GameServerManager.gameCore = new BaccaratGameCore();
        }
    }
