using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using System;
using UnityEngine;
using System.Numerics;
using realtime_game.Shared.Interfaces.StreamingHubs;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class RoomModel : BaseModel,IRoomHubReceiver
{
    private GrpcChannelx channel;
    private IRoomHub roomHub;

    //　接続ID
    public Guid ConnectionId { get; set; }

    //　ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    //ユーザーの退出通知
    public Action<Guid> OnLeavedUser { get; set; }

    //ユーザーの位置通知
    public Action<Guid,Vector3,Quaternion> OnMoveCharacter { get; set; }

    //　MagicOnion接続処理
    public async UniTask ConnectAsync()
    {
        channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.
             ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
        this.ConnectionId = await roomHub.GetConnectionId();
    }

    //　MagicOnion切断処理
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    //　破棄処理 
    async void OnDestroy()
    {
        DisconnectAsync();
    }

    //　入室
    public async UniTask JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userId);
        foreach (var user in users)
        {
            if (OnJoinedUser != null)
            {
                OnJoinedUser(user);
            }

            Debug.Log(user.ConnectionId);
            Debug.Log(user.UserData.Id);
            Debug.Log(user.UserData.Name);
        }
    }

    //　入室通知 (IRoomHubReceiverインタフェースの実装)
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser != null)
        {
            OnJoinedUser(user);
        }

        //Debug.Log(user.ConnectionId);
        //Debug.Log(user.UserData.Id);
        //Debug.Log(user.UserData.Name);
    }


    //退出
    public async UniTask LeaveAsync()
    {

        Debug.Log("LeaveAsync");
        if (roomHub != null)
        {
            await roomHub.LeaveAsync();
        }
    }

    //退出通知
    public void OnLeave(Guid Id)
    {

        Debug.Log("OnLeave");
        if (OnLeavedUser != null)
        {
            OnLeavedUser(Id);
        }
    }

    //移動処理
    public async UniTask MoveAsync(Vector3 pos, Quaternion quaternion)
    {
        await roomHub.MoveAsync(pos, quaternion);
    }


    //移動通知

    public void OnMove(Guid connectionId, Vector3 pos,Quaternion quaternion)
    {
        if (connectionId == this.ConnectionId) return;

        if (OnMoveCharacter == null) return;

        OnMoveCharacter(connectionId, pos, quaternion);
    }
}

