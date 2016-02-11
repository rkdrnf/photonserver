using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

using Game.LGNetwork;
using System.Collections;
using Game.Operations;
using ProtoBuf;
using System.IO;

namespace Game
{
    public abstract class GameCore : IGameCore
    {
        protected int MAX_ROOM_COUNT = 20;
        protected Dictionary<int, GameRoom> roomsDic;

        protected Dictionary<int, GamePeer> peersDic;
        protected Dictionary<int, PeerInfo> peerInfosDic;

        public virtual void Setup()
        {
        }

        public void TearDown()
        {
            foreach (var room in roomsDic.Values)
            {
                room.Dispose();
            }
        }

        public virtual void HandleOperationRequest(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
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

                case CommonOperationCode.ConfirmJoin:
                    HandleConfirmJoinOperation(peer, operationRequest, sendParameters);
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
            var joinRequest = new JoinRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(joinRequest, sendParameters))
            {
                return;
            }

            var room = FindRoom(joinRequest.RoomID);

            var response = new OperationResponse(CommonOperationCode.Join,
                   new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false } });
            response.Parameters[(byte)JoinParameterKey.RoomID] = joinRequest.RoomID;

            if (room != null && room.CanJoin(peer))
            {
                response.Parameters[(byte)CommonParameterKey.Success] = true;
            }
            peer.SendOperationResponse(response, sendParameters);

        }

        public void HandleConfirmJoinOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var joinRequest = new ConfirmJoinRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(joinRequest, sendParameters))
            {
                return;
            }

            ClearPeerInfo(peer);
            peersDic.Add(peer.ConnectionId, peer);
            peerInfosDic.Add(peer.ConnectionId, new PeerInfo(peer, joinRequest.RoomID ));

            var room = FindRoom(joinRequest.RoomID);

            var response = new OperationResponse(CommonOperationCode.ConfirmJoin,
                new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false } });
            if (room != null)
            {
                room.Join(peer, joinRequest, sendParameters);
            }
            else
            {
                peer.SendOperationResponse(response, sendParameters);
            }
        }


        public void HandleExitOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var exitRequest = new ExitRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(exitRequest, sendParameters))
            {
                return;
            }

            ClearPeerInfo(peer);
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

        public void HandleDisconnect(GamePeer peer)
        {
            ClearPeerInfo(peer);
        }

        public void SendAllRoomStatus(GamePeer peer, SendParameters sendParameters)
        {
            GetRoomsResponse response = new GetRoomsResponse();
            response.RoomProperties = PacketHelper.Serialize<RoomPropertyList>(GetRoomProperties());
            peer.SendOperationResponse(new OperationResponse(CommonOperationCode.GetRooms, response), sendParameters);
        }

        private RoomPropertyList GetRoomProperties()
        {
            RoomPropertyList roomProps;
            roomProps.Properties = new List<RoomProperty>();
            foreach (var room in roomsDic.Values)
            {
                roomProps.Properties.Add(room.GetProperty());
            }

            return roomProps;
        }

        protected GameRoom FindRoom(int roomID)
        {
            if (roomsDic.ContainsKey(roomID)) {
                return roomsDic[roomID];
            }
            else return null;
        }

        protected GameRoom FindPeerRoom(GamePeer peer)
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
