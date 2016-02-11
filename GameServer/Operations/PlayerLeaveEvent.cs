using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Game.Operations
{
    public class PlayerLeaveEvent
    {
        [DataMember(Code = (byte)PlayerLeavePK.Actor, IsOptional = false)]
        public int Actor { get; set; }
    }
}
