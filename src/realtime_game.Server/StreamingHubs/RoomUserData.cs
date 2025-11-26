using realtime_game.Shared.Interfaces.StreamingHubs;
using UnityEngine;

namespace realtime_game.Server.StreamingHubs
{
    public class RoomUserData
    {
        public JoinedUser JoinedUser;
        internal Vector3 pos;
        internal Quaternion quaternion;
    }
}
