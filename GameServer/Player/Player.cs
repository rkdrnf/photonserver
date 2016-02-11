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
        public string name;


        public Player(GamePeer peer, PlayerInfo info)
        {
            this.peer = peer;
            this.key = PlayerKey.MakeFromPeer(peer);
            this.name = info.username;
        }
    }
}
