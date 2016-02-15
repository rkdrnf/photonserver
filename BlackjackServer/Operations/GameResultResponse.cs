using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Blackjacks.Operations
{
    public class GameResultResponse
    {
        [DataMember(Code = (byte)BlackjackGameResultPK.GameResult, IsOptional = false)]
        public byte[] deckResults { get; set; }  //BlackjackGameResult
        [DataMember(Code = (byte)BlackjackGameResultPK.BetMoney, IsOptional = false)]
        public int BetMoney { get; set; }
        [DataMember(Code = (byte)BlackjackGameResultPK.DealerCards, IsOptional = false)]
        public byte[] DealerCards { get; set; } //CardSet
    }

    public enum BlackjackGameResultPK : byte
    {
        GameResult = 0,
        BetMoney = 1,
        DealerCards = 2
    }
}
