using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PeerInfo
    {
        public PeerInfo(GamePeer peer)
        {
            this.connectionID = peer.ConnectionId;
            this.roomID = -1;
        }

        public int connectionID;
        public int roomID;
    }
}
