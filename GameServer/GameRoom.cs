using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Game.Operations;
using ExitGames.Concurrency.Fibers;

namespace Game
{
    public abstract class GameRoom 
    {
        int id;
        protected PlayerManager playerManager;

        protected event Action<GamePeer, EventData, SendParameters> BroadcastMessageHandler;
        
        public delegate void OnPlayerLeaveAction(Player player);
        public delegate void OnPlayerJoinAction(Player player);

        protected event OnPlayerJoinAction OnPlayerJoinHandler;
        protected event OnPlayerLeaveAction OnPlayerLeaveHandler;

        protected readonly object syncRoot = new object();

        public PoolFiber ExecutionFiber { get; private set; }

        public GameRoom(int id)
        {
            this.id = id;
            ExecutionFiber = new PoolFiber();
            ExecutionFiber.Start();
        }

        public int GetID()
        {
            return id;
        }

        public bool CanJoin(GamePeer peer)
        {
            if (playerManager.HasPlayer(peer))
            {
                return false; //already joined.
            }

            if (playerManager.isFull())
            {
                return false; //exceed max player count.
            }

            return true;
        }

        public void Join(GamePeer peer, ConfirmJoinRequest joinReq, SendParameters sendParameters, PlayerInfo info)
        {
            if (CanJoin(peer))
            {
                Player newPlayer = playerManager.AddPlayer(peer, info);

                BroadcastMessageHandler += peer.OnBroadcastMessage;
                peer.OnLeaveHandler += OnPeerLeave;

                OnJoin(peer);
            }
            else
            {
                var response = new OperationResponse(CommonOperationCode.ConfirmJoin,
                new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, false } });

                peer.SendOperationResponse(response, sendParameters);
            }
        }

        protected abstract void OnJoin(GamePeer peer);

        public void Leave(GamePeer peer)
        {
            OnPeerLeave(peer);
        }

        private void OnPeerLeave(GamePeer peer)
        {
            RemovePlayer(peer, null, new SendParameters());
        }

        public virtual void RemovePlayer(GamePeer peer, ExitRequest exitReq, SendParameters sendParameters)
        {
            lock (syncRoot)
            { 
                if (playerManager.HasPlayer(peer) == false)
                {
                    return; //already removed.
                }

                BroadcastMessageHandler -= peer.OnBroadcastMessage;
                peer.OnLeaveHandler -= OnPeerLeave;

                PlayerLeaveEvent leaveEvent = new PlayerLeaveEvent();
                leaveEvent.Actor = playerManager.GetPlayer(peer).key.ID;

                EventData eventData = new EventData(EventCode.PlayerLeave, leaveEvent);
                BroadcastMessage(peer, eventData, sendParameters);

                playerManager.RemovePlayer(peer);

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
            prop.PlayerCount = playerManager.Count;
            prop.MaxPlayerCount = playerManager.MaxCount;

            return prop;
        }

        public bool HasPlayer(GamePeer peer)
        {
            return playerManager.HasPlayer(peer);
        }

        protected void BroadcastMessage(GamePeer peer, EventData eventData, SendParameters sendParameters)
        {
            if (BroadcastMessageHandler == null) return;

            BroadcastMessageHandler(peer, eventData, sendParameters);
        }

        public IDisposable ScheduleJob(Job job, int timems)
        {
            return ExecutionFiber.Schedule(() => this.ProcessJob(job), timems);
        }

        public void ProcessJob(Job job)
        {
            if (job == null)
            {
                return;
            }

            job.Run(this);
        }

        public void Dispose()
        {
            ExecutionFiber.Dispose();
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
