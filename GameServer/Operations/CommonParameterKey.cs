using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Operations
{
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

    //128~255
    public enum CommonParameterKey : byte
    {
        Success = 128
    }
}
