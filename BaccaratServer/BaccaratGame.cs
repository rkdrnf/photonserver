using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Operations;
using Casino;
using Baccarats.Operations;

namespace Baccarats
{
    public class BaccaratGame
    {
        private List<IDisposable> gameLifeSchedules;

        private CardSet bankerCards;
        private CardSet playerCards;

        private int bankerScore;
        private int playerScore;
        private BaccaratResultType result;

        public BaccaratGame()
        {
            gameLifeSchedules = new List<IDisposable>();
            bankerCards.cards = new List<Casino.Card>();
            playerCards.cards = new List<Casino.Card>();
        }

        public void AddSchedule(IDisposable schedule)
        {
            gameLifeSchedules.Add(schedule);
        }

        public void Clear()
        {
            bankerCards.cards.Clear();
            playerCards.cards.Clear();
            foreach(var schedule in gameLifeSchedules)
            {
                schedule.Dispose();
            }
            gameLifeSchedules.Clear();
        }

        public void Start()
        {
            DetermineCards();
        }

        private void DetermineCards()
        {
            var random = new System.Random();

            // Pick 2 cards for each
            bankerCards.cards.Add(PickRandomCard(random));
            bankerCards.cards.Add(PickRandomCard(random));
            playerCards.cards.Add(PickRandomCard(random));
            playerCards.cards.Add(PickRandomCard(random));

            // Get the scores for the two cards
            bankerScore = 0;
            foreach (var card in bankerCards.cards)
            {
                bankerScore += card.GetBaccaratScore();
            }
            bankerScore %= 10;

            playerScore = 0;
            foreach (var card in playerCards.cards)
            {
                playerScore += card.GetBaccaratScore();
            }
            playerScore %= 10;

            // Pick an extra card for each players if needed
            if (bankerScore >= 8 || playerScore >= 8)
            {
                //Natural
            }
            else if (playerScore >= 6)
            {
                //Player Stand
                if (bankerScore < 6)
                {
                    bankerCards.cards.Add(PickRandomCard(random));
                }
            }
            else
            {
                Card thirdPlayerCard = PickRandomCard(random);
                playerCards.cards.Add(thirdPlayerCard);

                bool anotherBankerCard = false;
                if (bankerScore <= 2)
                {
                    anotherBankerCard = true;
                }
                else if (bankerScore == 3)
                {
                    anotherBankerCard = thirdPlayerCard.GetBaccaratScore() != 8;
                }
                else if (bankerScore == 4)
                {
                    anotherBankerCard = thirdPlayerCard.GetBaccaratScore() >= 2 && thirdPlayerCard.GetBaccaratScore() <= 7;
                }
                else if (bankerScore == 5)
                {
                    anotherBankerCard = thirdPlayerCard.GetBaccaratScore() >= 4 && thirdPlayerCard.GetBaccaratScore() <= 7;
                }
                else if (bankerScore == 6)
                {
                    anotherBankerCard = thirdPlayerCard.GetBaccaratScore() == 6 || thirdPlayerCard.GetBaccaratScore() == 7;
                }

                if (anotherBankerCard)
                {
                    bankerCards.cards.Add(PickRandomCard(random));
                }
            }

            bankerScore = 0;
            playerScore = 0;

            foreach (var c in bankerCards.cards)
            {
                bankerScore += c.GetBaccaratScore();
            }
            bankerScore %= 10;

            foreach (var c in playerCards.cards)
            {
                playerScore += c.GetBaccaratScore();
            }
            playerScore %= 10;

            if (bankerScore > playerScore)
            {
                result = BaccaratResultType.Banker;
            }
            else if (bankerScore < playerScore)
            {
                result = BaccaratResultType.Player;
            }
            else
            {
                result = BaccaratResultType.Tie;
            }
        }

        public GameResultResponse GetBetResult(BaccaratBet bet)
        {
            int money = 0;
            if (result == BaccaratResultType.Banker)
            {
                money = bet.BankerBet * 95 / 100 - bet.PlayerBet
                    - bet.TieBet;
            }
            else if (result == BaccaratResultType.Player)
            {
                money = bet.PlayerBet - bet.BankerBet - bet.TieBet;
            }
            else
            {
                money = bet.TieBet * 8;
            }

            int betMoney = bet.BankerBet + bet.PlayerBet + bet.TieBet;


            BaccaratResult betResult = new BaccaratResult(result, money, betMoney);
            GameResultResponse response = new GameResultResponse();
            response.BankerCards = PacketHelper.Serialize(bankerCards);
            response.PlayerCards = PacketHelper.Serialize(playerCards);
            response.GameResult = (byte)betResult.Type;
            response.BetMoney = betResult.BetMoney;
            response.MoneyDelta = betResult.Money;

            return response;
        }

        private Card PickRandomCard(System.Random random)
        {
            Array ranks = Enum.GetValues(typeof(Rank));
            Array suits = Enum.GetValues(typeof(Suit));

            Rank rank = (Rank)ranks.GetValue(random.Next(ranks.Length));
            Suit suit = (Suit)suits.GetValue(random.Next(suits.Length));

            return new Card(rank, suit);
        }
    }
}
