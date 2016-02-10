using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PeerInfo
    {
        public PeerInfo(GamePeer peer, int roomID)
        {
            this.connectionID = peer.ConnectionId;
            this.roomID = roomID;
        }

        public int connectionID;
        public int roomID;
    }
}
