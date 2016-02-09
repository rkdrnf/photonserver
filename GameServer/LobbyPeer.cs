using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using GameServer.LGNetwork;

namespace GameServer
{
    public class LobbyPeer : PeerBase
    {
        public LobbyPeer(InitRequest initRequest) : base(initRequest)
        {
            
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case LGOperationCode.GetRooms:
                    GameServerManager.Instance.SendAllRoomStatus(this, sendParameters);
                    break;
            }
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            throw new NotImplementedException();
        }
    }
}