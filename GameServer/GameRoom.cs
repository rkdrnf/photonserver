using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Game;
using System.Collections;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Game
{
    public abstract class GameRoom : IDisposable
    {
        int id;
        private Dictionary<PlayerKey, Player> playersDic;

        private static readonly int MAX_PLAYER_COUNT = 7;

        private event Action<GamePeer, EventData, SendParameters> BroadcastMessage;
        private readonly object syncRoot = new object();

        public GameRoom(int id)
        {
            this.id = id;
            playersDic = new Dictionary<PlayerKey, Player>();
        }

        public int GetID()
        {
            return id;
        }

        public bool HasPlayer(GamePeer peer)
        {
            PlayerKey pKey;
            pKey.ID = peer.ConnectionId;

            if (playersDic.ContainsKey(pKey))
            {
                return true;
            }
            return false;
        }

        public bool CanJoin(GamePeer peer)
        {
            lock (syncRoot)
            { 
                if (HasPlayer(peer))
                {
                    return false; //already joined.
                }

                if (playersDic.Count == MAX_PLAYER_COUNT)
                {
                    return false; //exceed max player count.
                }

                return true;
            }

            
        }

        public void Join(GamePeer peer, ConfirmJoinRequest joinReq, SendParameters sendParameters)
        {
            if (CanJoin(peer))
            {
                PlayerKey pKey;
                pKey.ID = peer.ConnectionId;

                playersDic.Add(pKey, new Player(pKey));

                BroadcastMessage += peer.OnBroadcastMessage;

                var response = new OperationResponse(CommonOperationCode.ConfirmJoin,
                new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false },
                { (byte)ConfirmJoinParameterKey.RoomID, joinReq.RoomID } });

                peer.SendOperationResponse(response, sendParameters);
            }
            else
            {
                var response = new OperationResponse(CommonOperationCode.ConfirmJoin,
                new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false } });

                peer.SendOperationResponse(response, sendParameters);
            }

            
        }

        public void RemovePlayer(GamePeer peer, ExitRequest exitReq, SendParameters sendParameters)
        {
            lock (syncRoot)
            { 
                if (HasPlayer(peer) == false)
                {
                    return; //already removed.
                }

                PlayerKey pKey;
                pKey.ID = peer.ConnectionId;

                playersDic.Remove(pKey);

                BroadcastMessage -= peer.OnBroadcastMessage;
            }

            if (exitReq != null)
            { 
                var response = new OperationResponse(CommonOperationCode.Exit,
                    new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, true } });
                peer.SendOperationResponse(response, sendParameters);
            }
        }

        public void Chat(GamePeer peer, ChatRequest chatReq, SendParameters sendParameters)
        {
            var eventData = new EventData(EventCode.Chat) { Parameters = chatReq.OperationRequest.Parameters };
            lock (syncRoot)
            {
                BroadcastMessage(peer, eventData, sendParameters);
            }

            var response = new OperationResponse(CommonOperationCode.Chat);
            peer.SendOperationResponse(response, sendParameters);
        }

        public RoomProperty GetProperty()
        {
            RoomProperty prop;
            prop.ID = id;
            prop.PlayerCount = playersDic.Count;
            prop.MaxPlayerCount = MAX_PLAYER_COUNT;

            return prop;
        }


        public void Dispose()
        {
            //disposing code
        }
    }

    public enum RoomPropertyType : byte
    {
        ID = 0,
        PlayerCount = 1,
        MaxPlayerCount = 2,
    }
}
