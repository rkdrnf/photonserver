using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player
    {
        public GamePeer peer;
        public PlayerKey key;


        public Player(GamePeer peer, PlayerKey key)
        {
            this.peer = peer;
            this.key = key;
        }
    }
}
