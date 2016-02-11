using System.Collections;

namespace Casino
{

    public class BaccaratBet
    {

        public int BankerBet { get; private set; }
        public int PlayerBet { get; private set; }
        public int TieBet { get; private set; }

        public BaccaratBet(int bankerBet, int playerBet, int tieBet)
        {
            BankerBet = bankerBet;
            PlayerBet = playerBet;
            TieBet = tieBet;
        }
    }
}