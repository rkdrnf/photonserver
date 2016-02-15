using System.Collections;
using System.Collections.Generic;

namespace Casino
{

    public enum BlackjackResultType
    {
        Win, Lose, Push, Blackjack
    }

    public class BlackjackResult
    {

        public BlackjackResultType Type { get; set; }
        public int Money { get; set; }
        public int BetMoney { get; set; }

        public BlackjackResult()
        {
        }

        public BlackjackResult(BlackjackResultType type, int money, int betMoney)
        {
            Type = type;
            Money = money;
            BetMoney = betMoney;
        }
    }
}