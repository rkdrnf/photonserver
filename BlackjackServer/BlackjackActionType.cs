using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blackjacks
{
    public enum BlackjackActionType : byte
    {
        Stand = 0,
        Hit = 1,
        DoubleDown = 2,
        Split = 3
    }
}
