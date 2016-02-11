using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Baccarats
{
    public class NextGame : Job
    {
        public override void Run(object runObj)
        {
            BaccaratGameRoom room = runObj as BaccaratGameRoom;
            room.NextGame();
        }
    }
}
