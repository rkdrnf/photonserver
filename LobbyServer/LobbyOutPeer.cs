using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;

namespace LobbyServer
{
    public class LobbyOutPeer : ServerPeerBase
    {
        public LobbyOutPeer(IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
        : base(protocol, unmanagedPeer)
        {
        }
 
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // implement this if the outbound side should receive operation data
        }
 
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
        }
 
        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
           // implement this if the outbound side should receive events
        }
 
        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
        }
    }
}
