using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Operations
{
    //128~255
    public enum CommonParameterKey : byte
    {
        Success = 128
    }

    public enum JoinParameterKey : byte
    {
        RoomID = 0
    }

    public enum ExitParameterKey : byte
    {
        RoomID = 0
    }

    public enum ChatParameterKey : byte
    {
        Message = 0
    }

    public enum GetRoomsParameterKey : byte
    {
        RoomProperties = 0
    }

    public enum ConfirmJoinParameterKey : byte
    {
        RoomID = 0
    }

    public enum SendBetParameterKey : byte
    {
        BankerBet = 0,
        PlayerBet = 1,
        TieBet = 2
    }

    public enum BaccaratGameResultPK : byte
    {
        GameResult = 0,
        BetMoney = 1,
        MoneyDelta = 2,
        PlayerCards = 3,
        BankerCards = 4
    }
}
