﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace Game.Operations
{
    public class JoinRequest : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public JoinRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public JoinRequest()
        {
        }

        [DataMember(Code = (byte)JoinParameterKey.RoomID)]
        public int RoomID { get; set; }
    }
}
