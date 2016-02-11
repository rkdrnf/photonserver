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
        private static readonly int BACCARAT_PLAYER_COUNT = 7;
        private static readonly int MAX_GAME_TIME = 11000; //11secs

        private BaccaratGame game;

        public BaccaratGameRoom(int roomID)
            : base(roomID)
        {
            playersBetDic = new Dictionary<Player, BaccaratBet>();
            playerManager = new BaccaratPlayerManager(BACCARAT_PLAYER_COUNT);
            seatsDic = new Dictionary<int, Player>();
            game = new BaccaratGame();
        }

        private Dictionary<Player, BaccaratBet> playersBetDic;
        private Dictionary<int, Player> seatsDic;
        private int[] seatPriority = new int[] { 4, 3, 5, 2, 6, 1, 7 };

        public void Bet(GamePeer peer, SendBetRequest sendBet)
        {
            BaccaratPlayer player = playerManager.GetPlayer(peer) as BaccaratPlayer;
            var bet = new BaccaratBet(sendBet.BankerBet, sendBet.PlayerBet, sendBet.TieBet);

            if (HasBet(player) == false && CanBet(player, bet))
            {
                playersBetDic.Add(player, bet);
            }

            CheckAllBet();
        }

        public void BroadcastBet(GamePeer peer, BroadcastBetRequest broadcastBet)
        {
            broadcastBet.Actor = playerManager.GetPlayer(peer).key.ID;
            var eventData = new EventData(EventCode.BaccaratBroadcastBet) { Parameters = broadcastBet.OperationRequest.Parameters };
            
            lock (syncRoot)
            {
                BroadcastMessage(peer, eventData, new SendParameters());
            }

            var response = new OperationResponse(CommonOperationCode.BaccaratBroadcastBet, new Dictionary<byte, object> { { (byte)CommonParameterKey.Success, true } });
            peer.SendOperationResponse(response, new SendParameters());
        }

        private BaccaratBet GetBet(Player player)
        {
            if (playersBetDic.ContainsKey(player)) {
                return playersBetDic[player];
            } else {
                return null;
            }
        }

        private bool CanBet(BaccaratPlayer player, BaccaratBet bet)
        {
            return player.money >= (bet.BankerBet + bet.PlayerBet + bet.TieBet);
        }

        private void CheckAllBet()
        {
            if (playersBetDic.Count == playerManager.Count) //if all bet, send result cards set
            {
                NextGame();
            }
        }

        protected override void OnJoin(GamePeer peer)
        {
            BaccaratPlayer player = playerManager.GetPlayer(peer) as BaccaratPlayer;
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
                var bRoomPlayer = roomPlayer as BaccaratPlayer;
                if (bRoomPlayer.key.ID == player.key.ID) return;
                ExistingPlayerInfo pInfo;
                pInfo.ID = bRoomPlayer.key.ID;
                pInfo.Name = bRoomPlayer.name;
                pInfo.Seat = bRoomPlayer.seat;
                pInfo.Money = bRoomPlayer.money;
                pInfo.Bet = GetBet(bRoomPlayer);

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

        private void SitInEmptySeat(BaccaratPlayer player)
        {
            foreach(int seatNum in seatPriority)
            {
                if (seatsDic.ContainsKey(seatNum) == false)
                {
                    player.seat = seatNum;
                    seatsDic.Add(seatNum, player);
                }
            }
        }

        private void ClearGame()
        {
            playersBetDic.Clear();
            game.Clear();
        }

        private void EndGame()
        {
            SendResultToPlayers();
            //remove scheduled game;
        }

        private void StartGame()
        {
            var schedule = ScheduleJob(new NextGame(), MAX_GAME_TIME);
            game.AddSchedule(schedule);
            game.Start();
        }

        public void NextGame()
        {
            EndGame();
            ClearGame();
            StartGame();
        }

        private void SendResultToPlayers()
        {
            playerManager.ForEach((player) =>
            {
                BaccaratPlayer bPlayer = player as BaccaratPlayer;
                if (playersBetDic.ContainsKey(bPlayer))
                {
                    var bet = playersBetDic[bPlayer];
                    GameResultResponse response = game.GetBetResult(bet);
                    bPlayer.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
                    bPlayer.money += response.MoneyDelta;
                    //WebHelper.UpdatePlayerMoney(player.name, response.MoneyDelta);
                }
                else
                {
                    var bet = new BaccaratBet(0, 0, 0);
                    GameResultResponse response = game.GetBetResult(bet);
                    bPlayer.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
                }
            });
        }

        private bool HasBet(Player player)
        {
            return playersBetDic.ContainsKey(player);
        }

        public override void RemovePlayer(GamePeer peer, ExitRequest exitReq, SendParameters sendParameters)
        {
            Player player = playerManager.GetPlayer(peer);
            playersBetDic.Remove(player);

            base.RemovePlayer(peer, exitReq, sendParameters);

            CheckAllBet();
        }

    }
}
