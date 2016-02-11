using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Baccarats.Operations
{
    public class NewPlayerJoinEvent
    {
        [DataMember(Code = (byte)NewPlayerJoinPK.Actor, IsOptional = false)]
        public int Actor { get; set; }
        [DataMember(Code = (byte)NewPlayerJoinPK.Name, IsOptional = false)]
        public string Name { get; set; }
        [DataMember(Code = (byte)NewPlayerJoinPK.Money, IsOptional = false)]
        public int Money { get; set; }
        [DataMember(Code = (byte)NewPlayerJoinPK.Seat, IsOptional = false)]
        public int Seat { get; set; }
    }

    public enum NewPlayerJoinPK : byte
    {
        Actor = 0,
        Name = 1,
        Money = 2,
        Seat = 3
    }
}
