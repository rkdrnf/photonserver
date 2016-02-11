using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Baccarats
{
    public class BaccaratPlayer : Player
    {
        public BaccaratPlayer(GamePeer peer) : base(peer)
        {
            money = 0;
            win = 0;
            lose = 0;
        }

        int money;
        int win;
        int lose;
    }
}
