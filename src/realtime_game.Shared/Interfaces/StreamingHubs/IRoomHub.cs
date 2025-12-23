using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace realtime_game.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// クライアントから呼び出す処理を実装するクラス用インターフェース
    /// </summary>
    public interface IRoomHub : IStreamingHub<IRoomHub, IRoomHubReceiver>
    {
        Task<Guid> GetConnectionId();

        // [サーバーに実装]

        // [クライアントから呼び出す]

        // ユーザー入室
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        //ユーザー退出
        //Task<JoinedUser[]> LeaveAsync(string roomName, int userId);
        Task LeaveAsync();

        //ユーザー移動
        Task MoveAsync(Vector3 pos, Quaternion quaternion);

        //接触定判
        Task NotifyContactAsync(Guid targetConnectionId);
    }

}
