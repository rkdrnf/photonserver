using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class PlayerManager
    {
        protected Dictionary<PlayerKey, Player> playersDic;
        protected readonly int MAX_PLAYER_COUNT;

        protected readonly object playerLock = new object();

        public PlayerManager(int maxPlayerCount)
        {
            playersDic = new Dictionary<PlayerKey, Player>();
            MAX_PLAYER_COUNT = maxPlayerCount;
        }

        public Player GetPlayer(GamePeer peer)
        {
            PlayerKey key = PlayerKey.MakeFromPeer(peer);
            if (playersDic.ContainsKey(key))
            {
                return playersDic[key];
            }

            return null;
        }

        public bool HasPlayer(GamePeer peer)
        {
            PlayerKey pKey;
            pKey.ID = peer.ConnectionId;

            if (playersDic.ContainsKey(pKey))
            {
                return true;
            }
            return false;
        }

        public void AddPlayer(GamePeer peer)
        {
            lock (playerLock)
            { 
                Player newPlayer = MakePlayer(peer);
                playersDic.Add(newPlayer.key, newPlayer);
            }
        }

        public void RemovePlayer(GamePeer peer)
        {
            lock (playerLock)
            { 
                PlayerKey pKey = PlayerKey.MakeFromPeer(peer);
                playersDic.Remove(pKey);
            }
        }

        public int MaxCount
        {
            get { return MAX_PLAYER_COUNT; }
        }

        public int Count
        {
            get { return playersDic.Count; }
        }

        public bool isFull()
        {
            return playersDic.Count == MAX_PLAYER_COUNT;
        }

        public delegate void forEachAction(Player player);

        public void ForEach(forEachAction action)
        {
            foreach(var player in playersDic.Values)
            {
                action(player);
            }
        }

        protected abstract Player MakePlayer(GamePeer peer);
    }
}
