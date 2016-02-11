using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using PhotonHostRuntimeInterfaces;
using Photon.SocketServer.Rpc;

namespace Game
{
    public abstract class GamePeer : PeerBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public delegate void OnLeaveAction(GamePeer peer);

        public event OnLeaveAction OnLeaveHandler;
        
        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            GameServerManager.gameCore.HandleOperationRequest(this, operationRequest, sendParameters);
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            log.DebugFormat("GamePeer Disconnected. Reason: {0}, {1}", reasonCode, reasonDetail);
            GameServerManager.gameCore.HandleDisconnect(this);
        }

        public bool ValidateOperation(Operation operation, SendParameters sendParameters)
        {
            if (operation.IsValid)
            {
                return true;
            }

            string errorMessage = operation.GetErrorMessage();
            this.SendOperationResponse(new OperationResponse { OperationCode = operation.OperationRequest.OperationCode, ReturnCode = -1, DebugMessage = errorMessage }, sendParameters);
            return false;
        }

        public void OnBroadcastMessage(GamePeer peer, EventData eventData, SendParameters sendParameters)
        {
            if (peer != this)
            {
                this.SendEvent(eventData, sendParameters);
            }
        }

        public void Leave()
        {
            OnLeaveHandler(this);
        }

    }
}
