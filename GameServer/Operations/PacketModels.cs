using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace Game.Operations
{
    [ProtoContract]
    public struct RoomPropertyList
    {
        [ProtoMember(1)]
        public List<RoomProperty> Properties;
    }

    [ProtoContract]
    public struct RoomProperty
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public int PlayerCount;
        [ProtoMember(3)]
        public int MaxPlayerCount;
    }
}
