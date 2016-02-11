using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Casino;
using Game.Operations;

namespace Baccarats.Operations
{
    [ProtoContract]
    public struct ExistingPlayerInfos
    {
        [ProtoMember(1)]
        public List<ExistingPlayerInfo> infos;
    }

    [ProtoContract]
    public struct ExistingPlayerInfo
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public int Seat;
        [ProtoMember(4)]
        public int Money;
        [ProtoMember(5)]
        public BaccaratBet Bet;
    }
}
