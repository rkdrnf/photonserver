using System.Collections;
using ProtoBuf;

namespace Casino
{

    public enum Rank
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }

    public enum Suit
    {
        Spades, Hearts, Diamonds, Clubs
    }

    [ProtoContract]
    public class Card
    {
        [ProtoMember(1)]
        public Rank Rank { get; private set; }
        [ProtoMember(2)]
        public Suit Suit { get; private set; }

        public Card() { }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public int GetBaccaratScore()
        {
            switch (Rank)
            {
                case Rank.Ace:
                    return 1;
                case Rank.Two:
                    return 2;
                case Rank.Three:
                    return 3;
                case Rank.Four:
                    return 4;
                case Rank.Five:
                    return 5;
                case Rank.Six:
                    return 6;
                case Rank.Seven:
                    return 7;
                case Rank.Eight:
                    return 8;
                case Rank.Nine:
                    return 9;
                default:
                    return 0;
            }
        }

        public int GetBlackjackMinScore()
        {
            switch (Rank)
            {
                case Rank.Ace:
                    return 1;
                case Rank.Two:
                    return 2;
                case Rank.Three:
                    return 3;
                case Rank.Four:
                    return 4;
                case Rank.Five:
                    return 5;
                case Rank.Six:
                    return 6;
                case Rank.Seven:
                    return 7;
                case Rank.Eight:
                    return 8;
                case Rank.Nine:
                    return 9;
                default:
                    return 10;
            }
        }

        public bool Is10JQK()
        {
            return GetBlackjackMinScore() == 10;
        }

        public bool IsAce()
        {
            return Rank == Rank.Ace;
        }
    }
}