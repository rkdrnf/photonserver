using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Casino;

namespace Blackjacks
{
    [ProtoContract]
    public class BlackjackDeckResults
    {
        [ProtoMember(1)]
        public List<BlackjackResult> deckResults;
    }
}
