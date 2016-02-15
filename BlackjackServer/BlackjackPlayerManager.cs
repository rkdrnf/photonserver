using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using System.Net;
using System.Threading;
using System.Collections.Specialized;

namespace Blackjacks
{
    public class BlackjackPlayerManager : PlayerManager
    {
        public BlackjackPlayerManager(int maxPlayerCount) : base(maxPlayerCount)
        {
        }

        protected override Player MakePlayer(GamePeer peer, PlayerInfo info)
        {
            //Get Info from webserver
            BlackjackPlayer newPlayer = new BlackjackPlayer(peer, info);

            return newPlayer;
        }


        
    }
}
