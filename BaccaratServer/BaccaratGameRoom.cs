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
            game = new BaccaratGame();
        }

        private Dictionary<Player, BaccaratBet> playersBetDic;

        public void Bet(GamePeer peer, SendBetRequest sendBet)
        {
            Player player = playerManager.GetPlayer(peer);

            if (HasBet(player) == false && CanBet(player))
            {
                playersBetDic.Add(player, new BaccaratBet(sendBet.BankerBet, sendBet.PlayerBet, sendBet.TieBet));
            }

            CheckAllBet();
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

        private void CheckAllBet()
        {
            if (playersBetDic.Count == playerManager.Count) //if all bet, send result cards set
            {
                NextGame();
            }
        }

        protected override void OnJoin(GamePeer peer)
        {
            if (playerManager.Count == 1)
            {
                ClearGame();
                StartGame();
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
                if (playersBetDic.ContainsKey(player))
                {
                    var bet = playersBetDic[player];
                    GameResultResponse response = game.GetBetResult(bet);
                    player.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
                }
                else
                {
                    var bet = new BaccaratBet(0, 0, 0);
                    GameResultResponse response = game.GetBetResult(bet);
                    player.peer.SendOperationResponse(new OperationResponse(CommonOperationCode.BaccaratGameResult, response), new SendParameters());
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
