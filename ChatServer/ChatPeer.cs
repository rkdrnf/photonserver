using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

public class ChatPeer : PeerBase
{
    private static readonly object syncRoot = new object();

    private static event Action<ChatPeer, EventData, SendParameters> BroadcastMessage;

    public ChatPeer(IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
        : base(protocol, unmanagedPeer)
    {
        lock (syncRoot)
        {
            BroadcastMessage += this.OnBroadcastMessage;
        }
    }

    protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
    {
        lock (syncRoot)
        {
            BroadcastMessage -= this.OnBroadcastMessage;
        }
    }

    protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
    {
        var @event = new EventData(1) { Parameters = operationRequest.Parameters };
        lock (syncRoot)
        {
            BroadcastMessage(this, @event, sendParameters);
        }

        var response = new OperationResponse(operationRequest.OperationCode);
        this.SendOperationResponse(response, sendParameters);
    }

    private void OnBroadcastMessage(ChatPeer peer, EventData @event, SendParameters sendParameters)
    {
        if (peer != this)
        {
            this.SendEvent(@event, sendParameters);
        }
    }
}


