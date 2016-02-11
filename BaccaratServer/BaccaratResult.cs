using System.Collections;

namespace Casino
{

    public enum BaccaratResultType
    {
        Banker, Player, Tie // Ommited win (i.e. Banker means "banker won the game")
    }

    public class BaccaratResult
    {

        public BaccaratResultType Type { get; private set; }
        public int Money { get; private set; }
        public int BetMoney { get; private set; }

        public BaccaratResult(BaccaratResultType type, int money, int betMoney)
        {
            Type = type;
            Money = money;
            BetMoney = betMoney;
        }
    }
}