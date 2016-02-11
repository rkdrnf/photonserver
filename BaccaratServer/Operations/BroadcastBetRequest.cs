using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

using Game.Operations;

namespace Baccarats
{
    public class BroadcastBetRequest : Operation
    {
        public BroadcastBetRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public BroadcastBetRequest()
        {
        }

        [DataMember(Code = (byte)SendBetParameterKey.BankerBet)]
        public int BankerBet { get; set; }

        [DataMember(Code = (byte)SendBetParameterKey.PlayerBet)]
        public int PlayerBet { get; set; }

        [DataMember(Code = (byte)SendBetParameterKey.TieBet)]
        public int TieBet { get; set; }
    }
}
