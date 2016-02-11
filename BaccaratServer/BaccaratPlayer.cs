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
        public BaccaratPlayer(GamePeer peer, PlayerInfo info) : base(peer, info)
        {
            money = info.money;
            win = 0;
            lose = 0;
        }

        public int money;
        public int win;
        public int lose;
        public int seat;
    }
}
