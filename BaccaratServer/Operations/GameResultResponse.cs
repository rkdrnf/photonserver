using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Baccarats.Operations
{
    public class GameResultResponse
    {
        [DataMember(Code = (byte)BaccaratGameResultPK.GameResult, IsOptional = false)]
        public byte GameResult { get; set; } 
        [DataMember(Code = (byte)BaccaratGameResultPK.BetMoney, IsOptional = false)]
        public int BetMoney { get; set; }
        [DataMember(Code = (byte)BaccaratGameResultPK.MoneyDelta, IsOptional = false)]
        public int MoneyDelta { get; set; }
        [DataMember(Code = (byte)BaccaratGameResultPK.PlayerCards, IsOptional = false)]
        public byte[] PlayerCards { get; set; } //CardSet
        [DataMember(Code = (byte)BaccaratGameResultPK.BankerCards, IsOptional = false)]
        public byte[] BankerCards { get; set; } //CardSet
    }
}
