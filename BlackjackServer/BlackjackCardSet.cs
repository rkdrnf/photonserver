using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

namespace Casino
{

    [ProtoContract]
    public class BlackjackCardSet
    {
        [ProtoMember(1)]
        public List<Card> Cards { get; private set; }
        [ProtoMember(2)]
        public bool DoubleDowned { get; private set; }
        [ProtoMember(3)]
        public bool Finished { get; private set; }

        public BlackjackCardSet()
        {
            Cards = new List<Card>();
            DoubleDowned = false;
            Finished = false;
        }
        public BlackjackCardSet(List<Card> cards)
        {
            Cards = cards;
            DoubleDowned = false;
            Finished = false;
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void RemoveCardAt(int index)
        {
            Cards.RemoveAt(index);
        }

        public void DoubleDown()
        {
            DoubleDowned = true;
        }

        public void Finish()
        {
            Finished = true;
        }
    }
}