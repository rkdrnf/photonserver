using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Game;

namespace Baccarats
{
    public class BaccaratPeer : GamePeer
    {
        public BaccaratPeer(InitRequest initRequest)
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
