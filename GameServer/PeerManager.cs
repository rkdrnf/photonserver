using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace Game
{
    public class PeerManager
    {
        protected Dictionary<int, GamePeer> peersDic;
        protected Dictionary<int, PeerInfo> peerInfosDic;

        public PeerManager()
        {
            peersDic = new Dictionary<int, GamePeer>();
            peerInfosDic = new Dictionary<int, PeerInfo>();
        }

        public void Setup()
        {
            
        }

        public void TearDown()
        {
            peersDic.Clear();
            peerInfosDic.Clear();
        }

        public void OnPeerLeave(GamePeer peer)
        {
            ClearPeerInfo(peer);
        }

        public void OnPeerJoin(GamePeer peer, PeerInfo info)
        {
            ClearPeerInfo(peer);

            peersDic.Add(peer.ConnectionId, peer);
            peerInfosDic.Add(peer.ConnectionId, info);

            peer.OnLeaveHandler += OnPeerLeave;
        }

        private void ClearPeerInfo(GamePeer peer)
        {
            if (peersDic.ContainsKey(peer.ConnectionId))
            {
                peersDic.Remove(peer.ConnectionId);
            }

            if (peerInfosDic.ContainsKey(peer.ConnectionId))
            {
                peerInfosDic.Remove(peer.ConnectionId);
            }
        }
    }
}
