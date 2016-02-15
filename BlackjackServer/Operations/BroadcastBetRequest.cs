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

        [DataMember(Code = (byte)BroadcastBetPK.Actor)]
        public int Actor { get; set; }

        [DataMember(Code = (byte)BroadcastBetPK.Bet)]
        public int Bet { get; set; }
    }

    public enum BroadcastBetPK : byte
    {
        Actor = 0,
        Bet = 1
    }

    
}
