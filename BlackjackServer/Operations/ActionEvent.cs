using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer.Rpc;
using Game.Operations;

namespace Blackjacks.Operations
{
    public class ActionEvent
    {
        [DataMember(Code = (byte)ActionEventPK.Actor, IsOptional = false)]
        public int Actor { get; set; }
        [DataMember(Code = (byte)ActionEventPK.Action, IsOptional = false)]
        public BlackjackActionType ActionType { get; set; }
        [DataMember(Code = (byte)ActionEventPK.DeckIndex, IsOptional = false)]
        public int DeckIndex { get; set; }
        [DataMember(Code = (byte)ActionEventPK.ActionCardSet, IsOptional = false)]
        public byte[] ActionCardSet { get; set; } //BlackjackCardSet
        [DataMember(Code = (byte)ActionEventPK.ActionCardSet, IsOptional = false)]
        public int SplitDeckIndex { get; set; }
        [DataMember(Code = (byte)ActionEventPK.ActionCardSet, IsOptional = false)]
        public byte[] SplitCardSet { get; set; } //BlackjackCardSet

    }

    public enum ActionEventPK : byte
    {
        Actor = 0,
        Action = 1,
        DeckIndex = 2,
        ActionCardSet = 3,
        SplitDeckIndex = 4,
        SplitCardSet = 5
    }
}
