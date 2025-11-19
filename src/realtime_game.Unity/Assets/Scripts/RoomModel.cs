using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using System;
using UnityEngine;
using realtime_game.Shared.Interfaces.StreamingHubs;

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
    public async UniTask LeaveAsync(string roomName, int userId)
    {

        Debug.Log("LeaveAsync");
        if (roomHub != null)
        {
            await roomHub.LeaveAsync(roomName, userId);
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
}

