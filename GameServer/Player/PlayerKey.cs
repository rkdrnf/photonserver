using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public struct PlayerKey
    {
        public int ID;

        public static PlayerKey MakeFromPeer(GamePeer peer)
        {
            PlayerKey key;
            key.ID = peer.ConnectionId;
            return key;
        }
    }
}
