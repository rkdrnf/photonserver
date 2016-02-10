using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Photon.SocketServer;


namespace Barcarrats
{ 
    public class BarcarratGameCore : GameCore
    {
        public void HandleOperationRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            base.HandleOperationRequest(gamePeer, operationRequest, sendParameters);
            //nothing
        }

        public override void Setup()
        {
            base.Setup();

            roomsDic = new Dictionary<int, GameRoom>();
            for (int i = 0; i < MAX_ROOM_COUNT; i++)
            {
                roomsDic.Add(i, new BarcarratGameRoom(i));
            }

            peersDic = new Dictionary<int, GamePeer>();
            peerInfosDic = new Dictionary<int, PeerInfo>();

        }
    }
}