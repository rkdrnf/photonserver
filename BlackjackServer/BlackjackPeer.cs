using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Game;

namespace Blackjacks
{
    public class BlackjackPeer : GamePeer
    {
        public BlackjackPeer(InitRequest initRequest)
            : base(initRequest)
        {
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            base.OnOperationRequest(operationRequest, sendParameters);
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            base.OnDisconnect(reasonCode, reasonDetail);
        }
    }
}
