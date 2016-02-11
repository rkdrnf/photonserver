using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Photon.SocketServer;
using Game.Operations;
using Casino;

namespace Baccarats
{ 
    public class BaccaratGameCore : GameCore
    {
        public override void HandleOperationRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            base.HandleOperationRequest(gamePeer, operationRequest, sendParameters);
            //nothing

            switch(operationRequest.OperationCode)
            {
                case CommonOperationCode.BaccaratBet:
                    HandleBetRequest(gamePeer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.BaccaratBroadcastBet:
                    HandleBetBroadcastRequest(gamePeer, operationRequest, sendParameters);
                    break;
            }

        }

        private void HandleBetRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var room = FindPeerRoom(gamePeer) as BaccaratGameRoom;

            var sendBetRequest = new SendBetRequest(gamePeer.Protocol, operationRequest);

            if (room != null)
            {
                room.Bet(gamePeer, sendBetRequest);
            }
        }

        private void HandleBetBroadcastRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var room = FindPeerRoom(gamePeer) as BaccaratGameRoom;

            var betBroadcast = new BroadcastBetRequest(gamePeer.Protocol, operationRequest);

            if (room != null)
            {
                room.BroadcastBet(gamePeer, betBroadcast);
            }
        }

        public override void Setup()
        {
            base.Setup();

            roomsDic = new Dictionary<int, GameRoom>();
            for (int i = 0; i < MAX_ROOM_COUNT; i++)
            {
                roomsDic.Add(i, new BaccaratGameRoom(i));
            }

            peersDic = new Dictionary<int, GamePeer>();
            peerInfosDic = new Dictionary<int, PeerInfo>();

        }
    }
}