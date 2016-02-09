using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Game;

    public abstract class GameServer : ApplicationBase
    {
        protected override abstract PeerBase CreatePeer(InitRequest initRequest);

        protected override void Setup()
        {
            SetupServerManager();
            GameServerManager.Setup();
        }

        protected override void TearDown()
        {
            GameServerManager.TearDown();
        }

        protected abstract void SetupServerManager();


    }
