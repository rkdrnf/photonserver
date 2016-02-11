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
using System.Net.Http;
using Newtonsoft.Json;

namespace Game
{
    public abstract class GameCore : IGameCore
    {
        protected int MAX_ROOM_COUNT = 20;
        protected Dictionary<int, GameRoom> roomsDic;

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
                    var task = HandleConfirmJoinOperation(peer, operationRequest, sendParameters);
                    int x = 5;
                    task.Wait();
                    break;
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

        public async Task HandleConfirmJoinOperation(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var joinRequest = new ConfirmJoinRequest(peer.Protocol, operationRequest);
            if (!peer.ValidateOperation(joinRequest, sendParameters))
            {
                return;
            }

            //string playerInfoStr = await WebHelper.RequestPlayerInfo(peer, joinRequest.UserKey);
            string playerInfoStr = "{ \"username\": \"test\", \"money\": 2000 }";

            PlayerInfo info = JsonConvert.DeserializeObject<PlayerInfo>(playerInfoStr);

            PeerManager.Instance.OnPeerJoin(peer, new PeerInfo(peer, joinRequest.RoomID, joinRequest.UserKey));

            var room = FindRoom(joinRequest.RoomID);

            if (room != null)
            {
                room.ExecutionFiber.Enqueue(() => room.Join(peer, joinRequest, sendParameters, info));
            }
            else
            {
                var response = new OperationResponse(CommonOperationCode.ConfirmJoin,
                new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false } });
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

            var room = FindPeerRoom(peer);
            if (room != null)
            {
                room.ExecutionFiber.Enqueue(() => room.Leave(peer));
            }
            peer.Leave();
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
            peer.Leave();
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
