using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Blackjacks
{
    public class EndGameJob : Job
    {
        public override void Run(object runObj)
        {
            BlackjackGameRoom room = runObj as BlackjackGameRoom;
            room.NextGame();
        }
    }
}
