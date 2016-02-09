using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

using Game.LGNetwork;
using System.Collections;
using Game.Operations;

namespace Game
{
    public abstract class GameCore : IGameCore
    {
        protected int MAX_ROOM_COUNT = 20;
        protected Dictionary<int, GameRoom> roomsDic;

        protected Dictionary<int, GamePeer> peersDic;
        protected Dictionary<int, PeerInfo> peerInfosDic;

        public abstract void Setup();
        /*
        {
            for (int i = 0; i < MAX_ROOM_COUNT; i++)
            {
                roomsDic.Add(i, new GameRoom(i));
            }
        }
         * */

        public void TearDown()
        {
            foreach (var room in roomsDic.Values)
            {
                room.Dispose();
            }
        }

        public void HandleOperationRequest(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case CommonOperationCode.Join:
                    HandleJoinOperation(peer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.Exit:
                    HandleExitOperation(peer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.Chat:
                    HandleChatOperation(peer, operationRequest, sendParameters);
                    break;

                case CommonOperationCode.GetRooms:
                    SendAllRoomStatus(peer, sendParameters);
                    break;
            }
        }

        private void ClearPeerInfo(GamePeer peer)
        {
            if (peersDic.ContainsKey(peer.ConnectionId))
            {
                peersDic.Remove(peer.ConnectionId);
            }

            if (peerInfosDic.ContainsKey(peer.ConnectionId))
            {
                var peerInfo = peerInfosDic[peer.ConnectionId];
                var room = FindRoom(peerInfo.roomID);
                if (room != null && room.HasPlayer(peer))
                {
                    room.RemovePlayer(peer, null, new SendParameters());
                }

                peerInfosDic.Remove(peer.ConnectionId);
            }
        }

        public void HandleJoinOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            ClearPeerInfo(peer);
            peersDic.Add(peer.ConnectionId, peer);
            peerInfosDic.Add(peer.ConnectionId, new PeerInfo(peer));

            var joinRequest = new JoinRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(joinRequest, sendParameters))
            {
                return;
            }

            var room = FindRoom(joinRequest.RoomID);
            if (room != null)
            {
                room.Join(peer, joinRequest, sendParameters);
            }
        }


        public void HandleExitOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var exitRequest = new ExitRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(exitRequest, sendParameters))
            {
                return;
            }

            var room = FindRoom(exitRequest.RoomID);
            if (room != null)
            {
                room.RemovePlayer(peer, exitRequest, sendParameters);
            }
        }

        public void HandleChatOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var chatRequest = new ChatRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(chatRequest, sendParameters))
            {
                return;
            }

            var room = FindPeerRoom(peer);
            if (room != null)
            {
                room.Chat(peer, chatRequest, sendParameters);
            }
        }

        public void SendAllRoomStatus(GamePeer peer, SendParameters sendParameters)
        {
            GetRoomsResponse response = new GetRoomsResponse();
            response.RoomProperties = GetRoomProperties();
            peer.SendOperationResponse(new OperationResponse(CommonOperationCode.GetRooms, response), sendParameters);
        }

        private List<Dictionary<byte, object>> GetRoomProperties()
        {
            List<Dictionary<byte, object>> roomProps = new List<Dictionary<byte, object>>();
            foreach (var room in roomsDic.Values)
            {
                roomProps.Add(room.GetProperty());
            }

            return roomProps;
        }

        private GameRoom FindRoom(int roomID)
        {
            if (roomsDic.ContainsKey(roomID)) {
                return roomsDic[roomID];
            }
            else return null;
        }

        private GameRoom FindPeerRoom(GamePeer peer)
        {
            foreach (var room in roomsDic.Values)
            {
                if (room.HasPlayer(peer))
                {
                    return room;
                }
            }

            return null;
        }

    }
}
