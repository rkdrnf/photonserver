using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using LobbyServer.Operations;

namespace LobbyServer
{
    class LobbyClientPeer : PeerBase
    {
        public LobbyClientPeer(InitRequest initRequest)
            : base(initRequest)
        { }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case OperationCode.Refresh:

                    break;
            }
            // implement this if the outbound side should receive operation data
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
        }
    }
}
