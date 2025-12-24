using MagicOnion.Server.Hubs;
using realtime_game.Server.Models.Contexts;
using realtime_game.Shared.Models.Entities;
using realtime_game.Shared.Interfaces.StreamingHubs;
using UnityEngine;

namespace realtime_game.Server.StreamingHubs
{
    public class RoomHub(RoomContextRepository roomContextRepository) : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        private RoomContextRepository roomContextRepos;
        private RoomContext roomContext;

        // ルームに接続
        public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)
        {
            // 同時に生成しないように排他制御
            lock (roomContextRepos)
            {
                // 指定の名前のルームがあるかどうかを確認
                this.roomContext = roomContextRepos.GetContext(roomName);
                if (this.roomContext == null)
                { // 無かったら生成
                    this.roomContext = roomContextRepos.CreateContext(roomName);
                }
            }

            // ルームに参加 & ルームを保持
            this.roomContext.Group.Add(this.ConnectionId, Client);

            // DBからユーザー情報取得
            GameDbContext context = new GameDbContext();
            User user = context.Users.Where(user => user.Id == userId).First();

            // 入室済みユーザーのデータを作成
            var joinedUser = new JoinedUser();
            joinedUser.ConnectionId = this.ConnectionId;
            joinedUser.UserData = user;

            Console.WriteLine("ConnectionId :" + joinedUser.ConnectionId);
            Console.WriteLine("UserId :" + user.Id);
            Console.WriteLine("UserName :" + user.Name);

            // ルームコンテキストにユーザー情報を登録
            var roomUserData = new RoomUserData() { JoinedUser = joinedUser };
            this.roomContext.RoomUserDataList[ConnectionId] = roomUserData;

            // 自分以外のルーム参加者全員に、ユーザーの入室通知を送信
            this.roomContext.Group.Except([this.ConnectionId]).OnJoin(joinedUser);

            // 入室リクエストをしたユーザーに、参加者の情報をリストで返す
            return this.roomContext.RoomUserDataList.Select(
                f => f.Value.JoinedUser).ToArray();
        }

        // 接続時の処理
        protected override ValueTask OnConnected()
        {
            roomContextRepos = roomContextRepository;
            return default;
        }

        //ルームから退出
        public Task LeaveAsync()
        {
            //　退室したことを全メンバーに通知
            this.roomContext.Group.All.OnLeave(this.ConnectionId);

            //　ルーム内のメンバーから自分を削除
            this.roomContext.Group.Remove(this.ConnectionId);

            //　ルームデータから退室したユーザーを削除
            this.roomContext.RoomUserDataList.Remove(this.ConnectionId);
            if (this.roomContext.RoomUserDataList.Count == 0)
            {
                roomContextRepos.RemoveContext(this.roomContext.Name);
            }
            return Task.CompletedTask;
        }


        // 切断時の処理
        protected override ValueTask OnDisconnected()
        {
            _ = LeaveAsync();
            return default;
        }

        // 接続ID取得
        public Task<Guid> GetConnectionId()
        {
            return Task.FromResult<Guid>(this.ConnectionId);
        }

        //移動処理
        public Task MoveAsync(Vector3 pos, Quaternion quaternion)
        {
            // 位置情報を記録
            this.roomContext.RoomUserDataList[this.ConnectionId].pos = pos;
            this.roomContext.RoomUserDataList[this.ConnectionId].quaternion = quaternion;

            // 移動情報を自分以外の全メンバーに通知
            this.roomContext.Group.Except([this.ConnectionId]).OnMove(this.ConnectionId, pos,quaternion);

            return Task.CompletedTask;

        }

        public async Task NotifyContactAsync(Guid targetConnectionId)
        {
            var myId = this.ConnectionId;

            // 本当に同じルームかチェック（重要）
            if (!roomContext.RoomUserDataList.ContainsKey(targetConnectionId))
                return;
            
            Console.WriteLine(targetConnectionId);

            // 全員 or 対象者に通知
            roomContext.Group.Except([this.ConnectionId]).OnContact(myId, targetConnectionId);
        }

        ////準備完了通知
        //public Task SetReadyAsync(bool isReady)
        //{
        //    Console.WriteLine($"[Server] SetReadyAsync {ConnectionId} : {isReady}");

        //    var user = roomContext.RoomUserDataList[this.ConnectionId];
        //    user.IsReady = isReady;

        //    // 全員に通知
        //    roomContext.Group.Except([this.ConnectionId]).OnReady(this.ConnectionId, isReady);

        //    // 全員準備完了チェック
        //    bool allReady = roomContext.RoomUserDataList.Values
        //        .All(u => u.IsReady);

        //    if (allReady)
        //    {
        //        roomContext.Group.All.OnAllReady();
        //    }

        //    return Task.CompletedTask;
        //}

        //準備完了通知
        public Task SetReadyAsync(bool isReady)
        {
            Console.WriteLine($"[Server] SetReadyAsync {ConnectionId} : {isReady}");

            var user = roomContext.RoomUserDataList[this.ConnectionId];
            user.IsReady = isReady;

            // Ready状態を全員に通知
            roomContext.Group.All.OnReady(this.ConnectionId, isReady);

            bool allReady = roomContext.RoomUserDataList.Values.All(u => u.IsReady);

            // 全員Ready & まだカウントダウンしてない
            if (allReady && !roomContext.IsCountingDown)
            {
                roomContext.IsCountingDown = true;
                roomContext.Group.All.OnCountdownStart(5);
            }

            // Ready解除された & カウントダウン中
            if (!isReady && roomContext.IsCountingDown)
            {
                roomContext.IsCountingDown = false;
                roomContext.Group.All.OnCountdownCancel();
            }

            return Task.CompletedTask;
        }
    }
}

