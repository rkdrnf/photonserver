using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Game.LGNetwork;
using ExitGames.Logging.Log4Net;
using ExitGames.Logging;

namespace Game
{
    public class LobbyPeer : PeerBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public LobbyPeer(InitRequest initRequest) : base(initRequest)
        {
            
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case LGOperationCode.GetRooms:
                    //GameServerManager.gameCore.SendAllRoomStatus(this, sendParameters);
                    break;
            }
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            log.DebugFormat("LobbyPeer Disconnected. Reason: {0}, {1}", reasonCode, reasonDetail);
        }
    }
}