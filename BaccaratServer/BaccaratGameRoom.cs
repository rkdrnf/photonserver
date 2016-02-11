using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Casino;
using Baccarats.Operations;
using Game.Operations;
using Photon.SocketServer;

namespace Baccarats
{
    public class BaccaratGameRoom : GameRoom
    {
        public BaccaratGameRoom(int roomID)
            : base(roomID)
        {
            playersBetDic = new Dictionary<Player, BaccaratBet>();
        }

        private Dictionary<Player, BaccaratBet> playersBetDic;

        public void Bet(GamePeer peer, SendBetRequest sendBet)
        {
            Player player = GetPlayer(peer);

            if (HasBet(player) == false && CanBet(player))
            {
                playersBetDic.Add(player, new BaccaratBet(sendBet.BankerBet, sendBet.PlayerBet, sendBet.TieBet));
            }

            CheckAllBet(player, sendBet);
        }

        public void BroadcastBet(GamePeer peer, BroadcastBetRequest broadcastBet)
        {
            var eventData = new EventData(EventCode.BaccaratBroadcastBet) { Parameters = broadcastBet.OperationRequest.Parameters };
            
            lock (syncRoot)
            {
                BroadcastMessage(peer, eventData, new SendParameters());
            }

            var response = new OperationResponse(CommonOperationCode.BaccaratBroadcastBet, new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, true } });
            peer.SendOperationResponse(response, new SendParameters());
        }

        private bool CanBet(Player player)
        {
            //check if player have enough money.
            return true;
        }

        private void CheckAllBet(Player player, SendBetRequest sendBet)
        {
            if (playersBetDic.Count == playersDic.Count) //if all bet, send result cards set
            {
                SendResultToPlayers();
                playersBetDic.Clear();
            }
        }

        private void SendResultToPlayers()
        {
            foreach (Player player in playersDic.Values)
            {
                var bet = playersBetDic[player];
                GameResultResponse response = Determine(bet);
                player.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
            }
        }

        private bool HasBet(Player player)
        {
            return playersBetDic.ContainsKey(player);
        }

        public GameResultResponse Determine(BaccaratBet bet)
        {
            // Pick 2 cards for each
            CardSet bankerCards;
            bankerCards.cards = new List<Card>();
            CardSet playerCards;
            playerCards.cards = new List<Card>();

            var random = new System.Random();

            bankerCards.cards.Add(PickRandomCard(random));
            bankerCards.cards.Add(PickRandomCard(random));
            playerCards.cards.Add(PickRandomCard(random));
            playerCards.cards.Add(PickRandomCard(random));

            // Get the scores for the two cards
            int bankerScore = 0;
            foreach (var card in bankerCards.cards)
            {
                bankerScore += card.GetBaccaratScore();
            }
            bankerScore %= 10;

            int playerScore = 0;
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

            BaccaratResult result = GetResult(bet, bankerCards, playerCards);

            GameResultResponse response = new GameResultResponse();
            response.GameResult = (byte)result.Type;
            response.BetMoney = result.BetMoney;
            response.MoneyDelta = result.Money;
            response.PlayerCards = PacketHelper.Serialize<CardSet>(playerCards);
            response.BankerCards = PacketHelper.Serialize<CardSet>(bankerCards);

            return response;
        }

        // Get the result from the cards
        private BaccaratResult GetResult(BaccaratBet bet, CardSet bankerCards, CardSet playerCards)
        {
            int bankerScore = 0;
            int playerScore = 0;

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

            BaccaratResultType type;

            if (bankerScore > playerScore)
            {
                type = BaccaratResultType.Banker;
            }
            else if (bankerScore < playerScore)
            {
                type = BaccaratResultType.Player;
            }
            else
            {
                type = BaccaratResultType.Tie;
            }

            int money = 0;

            if (type == BaccaratResultType.Banker)
            {
                money = bet.BankerBet * 95 / 100 - bet.PlayerBet
                    - bet.TieBet;
            }
            else if (type == BaccaratResultType.Player)
            {
                money = bet.PlayerBet - bet.BankerBet - bet.TieBet;
            }
            else
            {
                money = bet.TieBet * 8;
            }

            int betMoney = bet.BankerBet + bet.PlayerBet + bet.TieBet;

            return new BaccaratResult(type, money, betMoney);
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
