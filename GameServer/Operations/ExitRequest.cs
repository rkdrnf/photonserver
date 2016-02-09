using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace Game.Operations
{
    public class ExitRequest : Operation
    {
        public ExitRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public ExitRequest()
        {
        }

        [DataMember(Code = (byte)ExitParameterKey.RoomID)]
        public int RoomID { get; set; }
    }
}
