using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Casino;
using Blackjacks.Operations;
using Game.Operations;
using Photon.SocketServer;

namespace Blackjacks
{
    public class BlackjackGameRoom : GameRoom
    {
        private static readonly int BLACKJACK_PLAYER_COUNT = 7;
        private static readonly int MAX_GAME_TIME = 20000; //11secs

        private List<IDisposable> gameLifeSchedules;
        private List<IDisposable> betLifeSchedules;

        public enum GameStatus { WAITING, BETTING, PLAYING };

        private GameStatus status;
 

        public BlackjackGameRoom(int roomID, int minBet, int maxBet)
            : base(roomID)
        {
            playerManager = new BlackjackPlayerManager(BLACKJACK_PLAYER_COUNT);
            seatsDic = new Dictionary<int, Player>();
            playerCards = new Dictionary<BlackjackPlayer, List<BlackjackCardSet>>();
            dealerCards.cards = new List<Casino.Card>();
            gameLifeSchedules = new List<IDisposable>();
            betLifeSchedules = new List<IDisposable>();
            playerBetsDic = new Dictionary<Player, BlackjackBet>();
            this.minimumBet = minBet;
            this.maximumBet = maxBet;
        }

        private int minimumBet;
        private int maximumBet;

        private CardSet dealerCards;
        private Dictionary<int, Player> seatsDic;
        private Dictionary<BlackjackPlayer, List<BlackjackCardSet>> playerCards;
        private Dictionary<Player, BlackjackBet> playerBetsDic;
        private int[] seatPriority = new int[] { 4, 3, 5, 2, 6, 1, 7 };

        public void Bet(GamePeer peer, SendBetRequest sendBet)
        {
            BlackjackPlayer player = playerManager.GetPlayer(peer) as BlackjackPlayer;
            BlackjackBet playerBet;
            playerBet.initialBet = sendBet.BetMoney;
            playerBet.totalBet = sendBet.BetMoney;

            if (HasBet(player) == false && CanBet(player, playerBet))
            {
                playerBetsDic.Add(player, playerBet);

                BroadcastBetDone(player);
            }

            CheckAllBet(false);
        }

        public void BroadcastBetDone(BlackjackPlayer player)
        {
            BlackjackBet bet = playerBetsDic[player];

            BetDoneEvent betDone = new BetDoneEvent();
            betDone.Actor = player.key.ID;
            betDone.BetMoney = bet.totalBet;
            var eventData = new EventData(EventCode.BlackjackBetDone, betDone);

            BroadcastMessage(player.peer, eventData, new SendParameters());

            var response = new OperationResponse(CommonOperationCode.BlackjackBet, new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, true } });
            player.peer.SendOperationResponse(response, new SendParameters());
        }

        public void BroadcastBet(GamePeer peer, BroadcastBetRequest broadcastBet)
        {
            broadcastBet.Actor = playerManager.GetPlayer(peer).key.ID;
            var eventData = new EventData(EventCode.BlackjackBroadcastBet, broadcastBet);

            lock (syncRoot)
            {
                BroadcastMessage(peer, eventData, new SendParameters());
            }

            var response = new OperationResponse(CommonOperationCode.BlackjackBroadcastBet, new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, true } });
            peer.SendOperationResponse(response, new SendParameters());
        }

        public bool HasBet(BlackjackPlayer player)
        {
            return playerBetsDic.ContainsKey(player);
        }

        public void CheckAllBet(bool forceStart)
        {
            bool allBet = true;
            playerManager.ForEach((player) =>
            {
                BlackjackPlayer bPlayer = player as BlackjackPlayer;
                if (playerBetsDic.ContainsKey(bPlayer))
                {
                    bPlayer.status = PlayerStatus.Playing;
                    SitInEmptySeat(bPlayer);
                }
                else
                {
                    allBet = false;
                    bPlayer.status = PlayerStatus.Waiting;
                }
            });

            if (allBet || (forceStart && playerBetsDic.Count >= 1))
            {
                StartBlackjack();
            }
        }


        public void OnPlayerAction(GamePeer peer, ActionRequest action)
        {
            if (peer.ValidateOperation(action, new SendParameters()) == false)
            {
                return;
            }

            BlackjackPlayer player = playerManager.GetPlayer(peer) as BlackjackPlayer;

            if (player.status != PlayerStatus.Playing) return;

            if (playerCards[player][action.DeckIndex].Finished) return;

            BlackjackBet bet = playerBetsDic[player];

            ActionEvent actionEvent = new ActionEvent();
            actionEvent.Actor = player.key.ID;
            actionEvent.ActionType = action.ActionType;
            actionEvent.DeckIndex = action.DeckIndex;

            switch (action.ActionType)
            {
                case BlackjackActionType.Hit:
                    PlayerHit(player, action.DeckIndex);
                    FillActionEventDeck(ref actionEvent, player, action.DeckIndex);
                    break;

                case BlackjackActionType.Split:
                    if (AddBet(player, bet.initialBet) == false) return;
                    Card originalDeckCard;
                    Card newDeckCard;
                    Split(player, action.DeckIndex, out originalDeckCard, out newDeckCard);
                    FillActionEventDeck(ref actionEvent, player, action.DeckIndex, playerCards[player].Count - 1);
                    break;

                case BlackjackActionType.Stand:
                    Finish(player, action.DeckIndex);
                    FillActionEventDeck(ref actionEvent, player, action.DeckIndex);
                    break;

                case BlackjackActionType.DoubleDown:
                    if (AddBet(player, bet.initialBet) == false) return;
                    DoubleDown(player, action.DeckIndex);
                    PlayerHit(player, action.DeckIndex);
                    FillActionEventDeck(ref actionEvent, player, action.DeckIndex);
                    break;
            }

            EventData eventData = new EventData(EventCode.BlackjackAction, actionEvent);
            BroadcastMessage(peer, eventData, new SendParameters());

            var response = new OperationResponse(CommonOperationCode.BlackjackAction, actionEvent);
            peer.SendOperationResponse(response, new SendParameters());

            PlayerActionDone(player, action.DeckIndex);
        }

        private void FillActionEventDeck(ref ActionEvent eventData, BlackjackPlayer player, int deckIndex, int splitIndex = -1)
        {
            eventData.ActionCardSet = PacketHelper.Serialize<BlackjackCardSet>(playerCards[player][deckIndex]);
            eventData.SplitDeckIndex = splitIndex;
            if (splitIndex != -1)
            {
                eventData.SplitCardSet = PacketHelper.Serialize<BlackjackCardSet>(playerCards[player][splitIndex]);
            }
        }

        public Card PlayerHit(BlackjackPlayer player, int deckIndex)
        {
            Card card = PickRandomCard(new System.Random());
            playerCards[player][deckIndex].AddCard(card);
            return card;
        }

        public void DoubleDown(BlackjackPlayer player, int deckIndex)
        {
            playerCards[player][deckIndex].DoubleDown();
        }

        public void Split(BlackjackPlayer player, int deckIndex, out Card card1, out Card card2)
        {
            //if (!IsSplittable(player)) return;

            Card splitted = playerCards[player][deckIndex].Cards[1];
            List<Card> splittedList = new List<Card>();
            splittedList.Add(splitted);
            playerCards[player].Add(new BlackjackCardSet(splittedList));
            playerCards[player][deckIndex].RemoveCardAt(1);

            var random = new System.Random();
            card1 = PickRandomCard(random);
            card2 = PickRandomCard(random);

            playerCards[player][deckIndex].AddCard(card1);
            playerCards[player][playerCards[player].Count - 1].AddCard(card2);
        }

        

        public void PlayerActionDone(BlackjackPlayer player, int deckIndex)
        {
            if (IsDoubleDowned(player, deckIndex) || IsPlayer21(player, deckIndex))
            {
                Finish(player, deckIndex);
                CheckAllPlayerFinished();
            }
            else if (IsPlayerBusted(player, deckIndex))
            {
                Finish(player, deckIndex);
                CheckAllPlayerFinished();
            }
        }
        public void Finish(BlackjackPlayer player, int deckIndex)
        {
            playerCards[player][deckIndex].Finish();
        }

        public void CheckAllPlayerFinished()
        {
            bool isFinished = true;
            playerManager.ForEach((player) =>
            {
                BlackjackPlayer bPlayer = player as BlackjackPlayer;
                for (int i = 0; i < playerCards[bPlayer].Count; i++)
                {
                    if (bPlayer.status == PlayerStatus.Playing && !IsFinished(bPlayer, i))
                    {
                        isFinished = false;
                        return;
                    }
                }
            });
            
            if (isFinished) 
            {
                DealerDraw();
                EndGame();
            }
        }

        private bool CanBet(BlackjackPlayer player, BlackjackBet bet)
        {
            if (bet.initialBet >= minimumBet && bet.initialBet <= maximumBet)
            { 
                return player.money >= bet.totalBet;
            }
            return false;
        }

        private bool AddBet(BlackjackPlayer player, int betAdd)
        {
            if (CanAddBet(player, betAdd))
            {
                BlackjackBet bet = playerBetsDic[player];
                bet.totalBet += betAdd;
                playerBetsDic[player] = bet;
                return true;
            }
            return false;
        }

        private bool CanAddBet(BlackjackPlayer player, int betAdd)
        {
            if (playerBetsDic.ContainsKey(player))
            {
                return player.money >= betAdd;
            }
            return false;
        }

        protected override void OnJoin(GamePeer peer)
        {
            BlackjackPlayer player = playerManager.GetPlayer(peer) as BlackjackPlayer;
            SitInEmptySeat(player);

            NewPlayerJoinEvent newJoin = new NewPlayerJoinEvent();
            newJoin.Actor = player.key.ID;
            newJoin.Money = player.money;
            newJoin.Name = player.name;
            newJoin.Seat = player.seat;

            EventData eventData = new EventData(EventCode.PlayerJoin,newJoin);
            BroadcastMessage(peer, eventData, new SendParameters());

            ConfirmJoinResponse joinRes = new ConfirmJoinResponse();
            joinRes.Actor = player.key.ID;
            joinRes.Seat = player.seat;
            joinRes.Money = player.money;
            ExistingPlayerInfos infos;
            infos.infos = new List<ExistingPlayerInfo>();

            playerManager.ForEach((roomPlayer) =>
            {
                var bRoomPlayer = roomPlayer as BlackjackPlayer;
                if (bRoomPlayer.key.ID == player.key.ID) return;
                ExistingPlayerInfo pInfo;
                pInfo.ID = bRoomPlayer.key.ID;
                pInfo.Name = bRoomPlayer.name;
                pInfo.Seat = bRoomPlayer.seat;
                pInfo.Money = bRoomPlayer.money;
                pInfo.Bet = playerBetsDic.ContainsKey(bRoomPlayer) ? playerBetsDic[bRoomPlayer] : new BlackjackBet();
                pInfo.Decks = player.status == PlayerStatus.Playing ? PacketHelper.Serialize<List<BlackjackCardSet>>(playerCards[bRoomPlayer]) : new byte[] { };
                pInfo.Status = bRoomPlayer.status;

                infos.infos.Add(pInfo);
            });
            joinRes.OtherPlayerInfos = PacketHelper.Serialize<ExistingPlayerInfos>(infos);

            var response = new OperationResponse(CommonOperationCode.ConfirmJoin, joinRes);

            peer.SendOperationResponse(response, new SendParameters());

            if (playerManager.Count == 1)
            {
                ClearGame();
                StartGame();
            }
        }

        private void SitInEmptySeat(BlackjackPlayer player)
        {
            foreach(int seatNum in seatPriority)
            {
                if (seatsDic.ContainsKey(seatNum) == false)
                {
                    player.seat = seatNum;
                    seatsDic.Add(seatNum, player);
                    return;
                }
            }
        }

        private void ClearGame()
        {
            foreach (var schedule in gameLifeSchedules)
            {
                schedule.Dispose();
            }
            foreach (var schedule in betLifeSchedules)
            {
                schedule.Dispose();
            }
            gameLifeSchedules.Clear();
            betLifeSchedules.Clear();
            playerBetsDic.Clear();
            playerCards.Clear();
            dealerCards.cards.Clear();
        }

        private void EndGame()
        {
            SendResultToPlayers();
            status = GameStatus.WAITING;
        }

        private void StartBlackjack()
        {
            foreach (var schedule in betLifeSchedules)
            {
                schedule.Dispose();
            }
            betLifeSchedules.Clear();

            var endSchedule = ScheduleJob(new EndGameJob(), MAX_GAME_TIME);
            gameLifeSchedules.Add(endSchedule);

            status = GameStatus.PLAYING;
        }

        private void StartGame()
        {
            var schedule = ScheduleJob(new EndBetJob(), MAX_GAME_TIME);
            betLifeSchedules.Add(schedule);
            FirstDraw();

            status = GameStatus.BETTING;
        }

        public void NextGame()
        {
            EndGame();
            ClearGame();
            StartGame();
        }

        public void EndBet()
        {
            CheckAllBet(true);
        }

        private void SendResultToPlayers()
        {
            playerManager.ForEach((player) =>
            {
                BlackjackPlayer bPlayer = player as BlackjackPlayer;
                if (bPlayer.status == PlayerStatus.Playing)
                {
                    BlackjackDeckResults deckResults = new BlackjackDeckResults();
                    deckResults.deckResults = new List<BlackjackResult>();
                    int moneyDelta = 0;
                    for(int i = 0; i < playerCards[bPlayer].Count; i++)
                    {
                        BlackjackResult result = GetGameResult(bPlayer, i, playerBetsDic[player].initialBet);
                        deckResults.deckResults.Add(result);
                        moneyDelta += result.Money;
                    }
                    
                    GameResultResponse response = new GameResultResponse();
                    response.deckResults = PacketHelper.Serialize<BlackjackDeckResults>(deckResults);
                    response.BetMoney = playerBetsDic[player].initialBet;
                    response.DealerCards = PacketHelper.Serialize<CardSet>(dealerCards);

                    bPlayer.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
                    bPlayer.money += moneyDelta;
                    //WebHelper.UpdatePlayerMoney(player.name, moneyDelta);
                }
                else
                {
                    GameResultResponse response = new GameResultResponse();
                    response.BetMoney = 0;
                    response.DealerCards = PacketHelper.Serialize<CardSet>(dealerCards);
                    bPlayer.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
                }
            });
        }

        public override void RemovePlayer(GamePeer peer, ExitRequest exitReq, SendParameters sendParameters)
        {
            BlackjackPlayer player = playerManager.GetPlayer(peer) as BlackjackPlayer;
            playerBetsDic.Remove(player);
            playerCards.Remove(player);
            seatsDic.Remove(player.seat);

            base.RemovePlayer(peer, exitReq, sendParameters);

            CheckAllBet(false);
        }

        private void FirstDraw()
        {
            // Pick 2 cards for each
            var random = new System.Random();

            dealerCards.cards.Add(PickRandomCard(random));
            dealerCards.cards.Add(PickRandomCard(random));

            playerManager.ForEach((player) => {
                BlackjackPlayer bPlayer = player as BlackjackPlayer;
                if (bPlayer.status == PlayerStatus.Waiting) return;

                playerCards[bPlayer].Add(new BlackjackCardSet()); ;
                BlackjackCardSet pCardSet = playerCards[bPlayer][0];
                pCardSet.AddCard(PickRandomCard(random));
                pCardSet.AddCard(PickRandomCard(random));
            });
        }

        private void DealerDraw()
        {
            var random = new System.Random();
            while (GetMinScore(dealerCards.cards) < 17)
            {
                dealerCards.cards.Add(PickRandomCard(random));
            }
        }

        private bool IsDoubleDowned(BlackjackPlayer player, int deckIndex)
        {
            return playerCards[player][deckIndex].DoubleDowned;
        }

        public bool IsFinished(BlackjackPlayer player, int deckIndex)
        {
            return playerCards[player][deckIndex].Finished;
        }

        public bool IsDealerBusted()
        {
            return IsBusted(dealerCards.cards);
        }

        public bool IsPlayerBusted(BlackjackPlayer player, int deckIndex)
        {
            return IsBusted(playerCards[player][deckIndex].Cards);
        }

        public bool IsPlayer21(BlackjackPlayer player, int deckIndex)
        {
            List<int> scores = GetPossibleScores(playerCards[player][deckIndex].Cards);

            foreach (int score in scores)
            {
                if (score == 21)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetDealerBestScore()
        {
            return GetBestScore(dealerCards.cards);
        }

        public int GetPlayerBestScore(BlackjackPlayer player, int deckIndex)
        {
            return GetBestScore(playerCards[player][deckIndex].Cards);
        }

        private Card PickRandomCard(System.Random random)
        {
            Array ranks = Enum.GetValues(typeof(Rank));
            Array suits = Enum.GetValues(typeof(Suit));

            Rank rank = (Rank)ranks.GetValue(random.Next(ranks.Length));
            Suit suit = (Suit)suits.GetValue(random.Next(suits.Length));

            return new Card(rank, suit);
        }

        private static bool IsBlackjack(List<Card> cards)
        {
            if (cards.Count != 2)
            {
                return false;
            }

            return (cards[0].Is10JQK() && cards[1].IsAce())
                || (cards[0].IsAce() && cards[1].Is10JQK());
        }

        private static bool IsBusted(List<Card> cards)
        {
            return GetMinScore(cards) > 21;
        }

        private static int GetMinScore(List<Card> cards)
        {
            int score = 0;
            foreach (var card in cards)
            {
                score += card.GetBlackjackMinScore();
            }
            return score;
        }

        private static int GetAceCount(List<Card> cards)
        {
            int count = 0;
            foreach (var card in cards)
            {
                if (card.IsAce())
                {
                    ++count;
                }
            }
            return count;
        }

        private static List<int> GetPossibleScores(List<Card> cards)
        {
            int minScore = GetMinScore(cards);
            int aceCount = GetAceCount(cards);

            List<int> list = new List<int>();

            for (int i = 0; i <= aceCount; ++i)
            {
                list.Add(minScore + 10 * i);
            }

            return list;
        }

        private static int GetBestScore(List<Card> cards)
        {
            int bestScore = 0;

            foreach (var score in GetPossibleScores(cards))
            {
                if (score <= 21 && score > bestScore)
                {
                    bestScore = score;
                }
            }

            return bestScore;
        }

        private bool IsSplittable(BlackjackPlayer player, int deckIndex)
        {
            if (playerCards[player][deckIndex].Cards.Count != 2)
            {
                return false;
            }

            if (playerCards[player][deckIndex].Cards[0].Is10JQK() && playerCards[player][deckIndex].Cards[1].Is10JQK())
            {
                return true;
            }

            return playerCards[player][deckIndex].Cards[0].Rank == playerCards[player][deckIndex].Cards[1].Rank;
        }


        public bool IsDealerBlackjack()
        {
            return IsBlackjack(dealerCards.cards);
        }

        public bool IsPlayerBlackjack(BlackjackPlayer player, int deckIndex)
        {
            return IsBlackjack(playerCards[player][deckIndex].Cards);
        }

        private BlackjackResult GetGameResult(BlackjackPlayer player, int deckIndex, int betMoney)
        {
            BlackjackResult result = new BlackjackResult();
            var money = 0;
            //TODO: use blackjack result
            if (IsDoubleDowned(player, deckIndex))
            {
                betMoney *= 2;
            }

            if (IsDealerBlackjack() && IsPlayerBlackjack(player, deckIndex))
            {
                money = 0;
                result.Type = BlackjackResultType.Push;
            }
            else if (IsDealerBlackjack())
            {
                money = -betMoney;
                result.Type = BlackjackResultType.Lose;
            }
            else if (IsPlayerBlackjack(player, deckIndex))
            {
                money = betMoney * 3 / 2;
                result.Type = BlackjackResultType.Blackjack;
            }
            else if (IsPlayerBusted(player, deckIndex))
            {
                money = -betMoney;
                result.Type = BlackjackResultType.Lose;
            }
            else if (IsDealerBusted())
            {
                money = betMoney;
                result.Type = BlackjackResultType.Win;
            }
            else
            {
                int dealerBestScore = GetDealerBestScore();
                int playerBestScore = GetPlayerBestScore(player, deckIndex);

                if (dealerBestScore < playerBestScore)
                {
                    money = betMoney;
                    result.Type = BlackjackResultType.Win;
                }
                else if (dealerBestScore > playerBestScore)
                {
                    money = -betMoney;
                    result.Type = BlackjackResultType.Lose;
                }
                else
                {
                    money = 0;
                    result.Type = BlackjackResultType.Push;
                }
            }

            result.Money = money;
            result.BetMoney = betMoney;
            return result;
        }
    }
}
