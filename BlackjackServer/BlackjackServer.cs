using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Photon.SocketServer;
using Blackjacks;

    public class BlackjackServer : GameServer
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (initRequest.LocalPort == 4520) //S2S Connection
            {
                return new LobbyPeer(initRequest);
            }

            return new BlackjackPeer(initRequest);
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
            GameServerManager.gameCore = new BlackjackGameCore();
        }
    }
