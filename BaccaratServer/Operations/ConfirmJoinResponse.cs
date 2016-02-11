using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Baccarats.Operations
{
    public class ConfirmJoinResponse
    {
        [DataMember(Code = (byte)ConfirmJoinResponsePK.Actor, IsOptional = false)]
        public int Actor { get; set; }
        [DataMember(Code = (byte)ConfirmJoinResponsePK.Seat, IsOptional = false)]
        public int Seat { get; set; }
        [DataMember(Code = (byte)ConfirmJoinResponsePK.Money, IsOptional = false)]
        public int Money { get; set; }
        [DataMember(Code = (byte)ConfirmJoinResponsePK.OtherPlayerInfos, IsOptional = false)]
        public byte[] OtherPlayerInfos { get; set; } //OtherPlayerInfos
    }

    public enum ConfirmJoinResponsePK : byte
    {
        Actor = 0,
        Seat = 1,
        Money = 2,
        OtherPlayerInfos = 3
    }
}
