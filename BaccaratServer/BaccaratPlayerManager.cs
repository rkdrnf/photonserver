using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using System.Net;
using System.Threading;
using System.Collections.Specialized;
using System.Net.Http;

namespace Baccarats
{
    public class BaccaratPlayerManager : PlayerManager
    {
        public BaccaratPlayerManager(int maxPlayerCount) : base(maxPlayerCount)
        {
        }

        protected override Player MakePlayer(GamePeer peer, PlayerInfo info)
        {
            //Get Info from webserver
            BaccaratPlayer newPlayer = new BaccaratPlayer(peer, info);

            return newPlayer;
        }


        
    }
}
