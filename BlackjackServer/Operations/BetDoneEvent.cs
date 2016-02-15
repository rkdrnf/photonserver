using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Blackjacks.Operations
{
    public class BetDoneEvent
    {
        [DataMember(Code = (byte)BetDonePK.Actor, IsOptional = false)]
        public int Actor { get; set; }
        [DataMember(Code = (byte)BetDonePK.BetMoney, IsOptional = false)]
        public int BetMoney { get; set; }
    }

    public enum BetDonePK : byte
    {
        Actor = 0,
        BetMoney = 1
    }
}
