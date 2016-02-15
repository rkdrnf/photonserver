using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

using Game.Operations;

namespace Blackjacks.Operations
{
    public class SendBetRequest : Operation
    {
        public SendBetRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public SendBetRequest()
        {
        }

        [DataMember(Code = (byte)SendBetPK.BetMoney)]
        public int BetMoney { get; set; }
    }

    public enum SendBetPK : byte
    {
        Actor = 0,
        BetMoney = 1
    }
}
