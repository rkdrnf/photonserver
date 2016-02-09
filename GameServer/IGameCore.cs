using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace Game
{
    public interface IGameCore
    {
        void HandleOperationRequest(GamePeer peer, OperationRequest operationRequest, SendParameters sendParameters);
    }
}
