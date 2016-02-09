﻿using System.Collections;
using Photon.SocketServer.Rpc;
using System.Collections.Generic;

namespace Game.Operations
{
    public class GetRoomsResponse
    {
        [DataMember(Code = (byte)GetRoomsParameterKey.RoomProperties, IsOptional = false)]
        public List<Dictionary<byte, object>> RoomProperties { get; set; }
    }
}
