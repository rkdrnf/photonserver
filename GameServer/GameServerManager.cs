using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using GameServer.LGNetwork;
using System.Collections;

namespace GameServer
{
    public class GameServerManager
    {
        private static GameServerManager instance;

        public static GameServerManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new GameServerManager();
                }
                return instance;
            }
        }

        int MAX_ROOM_COUNT = 20;
        private Dictionary<int, GameRoom> roomsDic;

        private GameServerManager() 
        {
            for (int i = 0; i < MAX_ROOM_COUNT; i++)
            {
                roomsDic.Add(i, new GameRoom(i));
            }
        }

        public void Setup() { }

        public void TearDown() { }
     

        public void SendAllRoomStatus(LobbyPeer peer, SendParameters sendParameters)
        {
            GetRoomsResponse response = new GetRoomsResponse();
            response.RoomProperties = this.GetRoomProperties();
            peer.SendOperationResponse(new OperationResponse(LGOperationCode.GetRooms, response), sendParameters);
        }

        private readonly Hashtable GetRoomProperties()
        {
            Hashtable roomProps = new Hashtable();
            foreach (var room in roomsDic.Values)
            {
                roomProps.Add(room.GetID(), room.GetProperty());
            }

            return roomProps;
        }
    }
}
