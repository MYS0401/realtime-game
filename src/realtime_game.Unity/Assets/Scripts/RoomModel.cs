using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using System;
using UnityEngine;
using System.Numerics;
using realtime_game.Shared.Interfaces.StreamingHubs;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

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

    //接触判定
    public Action<Guid, Guid> OnContactReceived { get; set; }

    //準備完了通知
    public Action<Guid, bool> OnReadyStateChangedReceived { get; set; }
    public Dictionary<Guid, bool> ReadyStates = new();

    //全員準備完了
    public Action OnAllReadyReceived;

    public Action<int> OnCountdownStartReceived { get; set; }
    public Action OnCountdownCancelReceived { get; set; }

    bool isDisconnecting = false;

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
        if (isDisconnecting) return;
        isDisconnecting = true;

        try
        {
            if (roomHub != null)
            {
                await roomHub.DisposeAsync();
                roomHub = null;
            }

            if (channel != null)
            {
                await channel.ShutdownAsync();
                channel = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"DisconnectAsync error: {e}");
        }
    }

    //　破棄処理 
    async void OnDestroy()
    {
        DisconnectAsync().Forget();
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
            ReadyStates[user.ConnectionId] = false;
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

    //接触判定
    public void OnContact(Guid fromId, Guid toId)
    {
        OnContactReceived?.Invoke(fromId, toId);
    }

    // 接触判定送信
    public async void NotifyContactAsync(Guid targetConnectionId)
    {
        await roomHub.NotifyContactAsync(targetConnectionId);
    }

    //準備完了通知
    public void OnReady(Guid connectionId, bool isReady)
    {
        Debug.Log($"OnReady received {connectionId} : {isReady}");

        ReadyStates[connectionId] = isReady;
        OnReadyStateChangedReceived?.Invoke(connectionId, isReady);
    }

    public async UniTask SetReadyAsync(bool isReady)
    {
        Debug.Log("SetReady");

        await roomHub.SetReadyAsync(isReady);

    }

    //全員準備完了通知
    public void OnAllReady()
    {
        OnAllReadyReceived?.Invoke();
    }

    public void OnCountdownStart(int seconds)
    {
        OnCountdownStartReceived?.Invoke(seconds);
    }

    public void OnCountdownCancel()
    {
        OnCountdownCancelReceived?.Invoke();
    }

}

