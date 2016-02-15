using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Operations;
using Casino;
using Blackjacks.Operations;

namespace Blackjacks
{
    public class BlackjackGame
    {
        private List<IDisposable> gameLifeSchedules;

        public BlackjackGame()
        {
            gameLifeSchedules = new List<IDisposable>();
            
            
        }

        public void AddSchedule(IDisposable schedule)
        {
            gameLifeSchedules.Add(schedule);
        }

        public void Clear()
        {
            
        }

        public void Start(IEnumerable<BlackjackCardSet> playerCards)
        {
        }
                       
    }
}
