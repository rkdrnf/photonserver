using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer.Game;
using System.Collections;

namespace GameServer
{
    public class GameRoom : IDisposable
    {
        int id;
        private Dictionary<PlayerKey, Player> playersDic;

        private static readonly int MAX_PLAYER_COUNT = 7;


        public GameRoom(int id)
        {
            this.id = id;
            playersDic = new Dictionary<PlayerKey, Player>();
        }

        public readonly int GetID()
        {
            return id;
        }

        public Hashtable GetProperty()
        {
            Hashtable props = new Hashtable();

            props.Add(RoomProperty.ID, id);
            props.Add(RoomProperty.PlayerCount, playersDic.Count);
            props.Add(RoomProperty.MaxPlayerCount, MAX_PLAYER_COUNT);

            return props;
        }



        public void Dispose()
        {
            //disposing code
        }
    }

    public enum RoomProperty : byte
    {
        ID = 0,
        PlayerCount = 1,
        MaxPlayerCount = 2,
    }
}
