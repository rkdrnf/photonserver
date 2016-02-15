using System.Collections;
using ProtoBuf;
namespace Casino
{
    [ProtoContract]
    public class BaccaratBet
    {
        [ProtoMember(1)]
        public int BankerBet { get; private set; }
        [ProtoMember(2)]
        public int PlayerBet { get; private set; }
        [ProtoMember(3)]
        public int TieBet { get; private set; }

        public BaccaratBet()
        { }

        public BaccaratBet(int bankerBet, int playerBet, int tieBet)
        {
            BankerBet = bankerBet;
            PlayerBet = playerBet;
            TieBet = tieBet;
        }
    }

    public enum SendBetPK : byte
    {
        Actor = 0,
        BankerBet = 1,
        PlayerBet = 2,
        TieBet = 3
    }
}