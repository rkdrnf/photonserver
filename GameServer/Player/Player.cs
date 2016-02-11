using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class Player
    {
        public GamePeer peer;
        public PlayerKey key;


        public Player(GamePeer peer)
        {
            this.peer = peer;
            this.key = PlayerKey.MakeFromPeer(peer);
        }
    }
}
