using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Baccarats
{
    public class BaccaratPlayerManager : PlayerManager
    {
        public BaccaratPlayerManager(int maxPlayerCount) : base(maxPlayerCount)
        {
        }

        protected override Player MakePlayer(GamePeer peer)
        {
            //Get Info from webserver
            BaccaratPlayer newPlayer = new BaccaratPlayer(peer);

            return newPlayer;
        }
    }
}
