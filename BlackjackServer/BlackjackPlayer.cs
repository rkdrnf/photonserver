using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Blackjacks
{
    public class BlackjackPlayer : Player
    {
        public BlackjackPlayer(GamePeer peer, PlayerInfo info) : base(peer, info)
        {
            money = info.game_money;
            win = 0;
            lose = 0;
            status = PlayerStatus.Waiting;
        }

        public int money;
        public int win;
        public int lose;
        public int seat;
        public PlayerStatus status;
    }

    public enum PlayerStatus
    {
        Waiting = 0,
        Playing = 1
    }
}
