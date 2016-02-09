using System.Collections;
using Photon.SocketServer.Rpc;

namespace GameServer.LGNetwork
{
    public class GetRoomsResponse
    {
        [DataMember(Code = (byte)LGParameterKey.RoomProperties, IsOptional = false)]
        public Hashtable RoomProperties { get; set; }
    }
}
