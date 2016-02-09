using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Game.LGNetwork;
using System.Collections;

namespace Game
{
    public class GameServerManager : IServerManager
    {
        public static GameCore gameCore;

        private GameServerManager() 
        { }

        public static void Setup() { gameCore.Setup();  }

        public static void TearDown() { gameCore.TearDown();  }

        public void HandleOperationRequest(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        { 
            gameCore.HandleOperationRequest(peer, operationRequest, sendParameters);
        }
    }
}
