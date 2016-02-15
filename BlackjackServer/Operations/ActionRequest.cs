using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Blackjacks
{
    public class ActionRequest : Operation
    {
        public ActionRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public ActionRequest()
        {
        }

        [DataMember(Code = (byte)ActionPK.ActionType)]
        public BlackjackActionType ActionType { get; set; }

        [DataMember(Code = (byte)ActionPK.DeckIndex)]
        public int DeckIndex { get; set; }
    }

    public enum ActionPK : byte
    {
        ActionType = 1,
        DeckIndex = 2
    }
}
