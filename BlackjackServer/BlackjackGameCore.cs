using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Photon.SocketServer;
using Game.Operations;
using Casino;
using Blackjacks.Operations;

namespace Blackjacks
{ 
    public class BlackjackGameCore : GameCore
    {
        public override void HandleOperationRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            base.HandleOperationRequest(gamePeer, operationRequest, sendParameters);
            //nothing

            switch(operationRequest.OperationCode)
            {
                case CommonOperationCode.BlackjackBet:
                    HandleBetRequest(gamePeer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.BlackjackBroadcastBet:
                    HandleBetBroadcastRequest(gamePeer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.BlackjackAction:
                    HandleActionRequest(gamePeer, operationRequest, sendParameters);
                    break;
            }

        }

        private void HandleActionRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var room = FindPeerRoom(gamePeer) as BlackjackGameRoom;

            var actionRequest = new ActionRequest(gamePeer.Protocol, operationRequest);

            if (room != null)
            {
                room.OnPlayerAction(gamePeer, actionRequest);
            }
        }

        private void HandleBetRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var room = FindPeerRoom(gamePeer) as BlackjackGameRoom;

            var sendBetRequest = new SendBetRequest(gamePeer.Protocol, operationRequest);

            if (room != null)
            {
                room.Bet(gamePeer, sendBetRequest);
            }
        }

        private void HandleBetBroadcastRequest(GamePeer gamePeer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var room = FindPeerRoom(gamePeer) as BlackjackGameRoom;

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
            int minBet = 10;
            int maxBet = 200;
            for (int i = 0; i < MAX_ROOM_COUNT; i++)
            {
                roomsDic.Add(i, new BlackjackGameRoom(i, minBet, maxBet));
            }
        }
    }
}