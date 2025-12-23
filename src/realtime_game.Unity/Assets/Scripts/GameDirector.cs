using DG.Tweening;
using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    //[SerializeField] RoomModel roomModel;
    [SerializeField] InputField InputField;
    bool connect1 = false;
    //bool connect2 = false;
    RoomModel roomModel;
    UserModel userModel;

    [SerializeField] Rigidbody rg;

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    int myUserId; //自分のユーザーID
    User myself; //自分のユーザー情報を保持

    bool justWarped = false;

    Guid myConnectionId;

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;

        //退出
        roomModel.OnLeavedUser += this.OnLeavedUser;

        //移動
        roomModel.OnMoveCharacter += this.OnMoveUser;

        //接触
        roomModel.OnContactReceived += this.OnContactReceived;

        //接続
        await roomModel.ConnectAsync();

        // 自分の ConnectionId を保存
        myConnectionId =  roomModel.ConnectionId;
    }

    public async void JoinRoom()
    {
        if (connect1)
        {
            myUserId = InputUserId();

            try
            {
                //ユーザー情報を取得
                myself = await userModel.GetUserAsync(myUserId);
            }
            catch (Exception e)
            {
                Debug.Log("GetUser failed");
                Debug.Log(e);
            }

            //入室
            await roomModel.JoinAsync("sampleRoom", myUserId);

            InvokeRepeating(nameof(Move), 1f, 0.1f);
        }
        
    }
    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        //Debug.Log("Connection...");

        //すでに表示済みのユーザーは追加しない
        if (characterList.ContainsKey(user.ConnectionId))
        {
            return;
        }

        //自分は追加しない
        if (user.UserData.Id == myUserId)
        {
            return;
        }

        GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
        characterObject.transform.position = new Vector3(0, 0, 0);

        var remotePlayer = characterObject.GetComponent<RemotePlayer>();
        remotePlayer.ConnectionId = user.ConnectionId;

        characterList[user.ConnectionId] = characterObject;  //フィールドで保持
    }

    public async void LeaveRoom()
    {
        if (connect1)
        {
            myUserId = InputUserId();

            try
            {
                //ユーザー情報を取得
                myself = await userModel.GetUserAsync(myUserId);
            }
            catch (Exception e)
            {
                Debug.Log("GetUser failed");
                Debug.Log(e);
            }

            CancelInvoke(nameof(Move));

            //退出
            await roomModel.LeaveAsync();

            //初期位置,回転に戻す
            //rg.position = Vector3.zero;
            //rg.rotation = Quaternion.identity;
      
            foreach (var obj in characterList.Values)
            {
                Destroy(obj);
            }
            characterList.Clear();
        }

    }

    //ユーザーが退出した時の処理
    private void OnLeavedUser(Guid Id)
    {
        Debug.Log($"[OnLeavedUser] Id: {Id}");

        if (characterList.ContainsKey(Id))
        {
            var obj = characterList[Id];
            Debug.Log($"Destroy Target: {obj}");

            Destroy(obj);
            characterList.Remove(Id);

            Debug.Log("[OnLeavedUser] Destroyed.");
        }
        else
        {
            Debug.LogWarning($"[OnLeavedUser] No Key: {Id}");
        }
    }

    // 自分以外のユーザーの移動を反映
    private void OnMoveUser(Guid connectionId, Vector3 pos, Quaternion quaternion)
    {
        if (connectionId == myConnectionId)
            return;

        // いない人は移動できない
        if (!characterList.ContainsKey(connectionId))
        {
            return;
        }

        characterList[connectionId].transform.DOKill();

        // DOTweenを使うことでなめらかに動く！
        characterList[connectionId].transform.DOMove(pos, 0.15f);

        characterList[connectionId].transform.DORotateQuaternion(quaternion, 0.15f);
        //characterList[connectionId].transform.position = pos;

    }

    public async void Move()
    {

       //Debug.Log("Move" + rg.transform.position + rg.transform.rotation);

        if (justWarped)
        {
            justWarped = false;
            return;
        }

        if (rg == null) return;

        await roomModel.MoveAsync(rg.transform.position,rg.transform.rotation);
    }


    public void InputText()
    {
        connect1 = true;
    }

    public int InputUserId()
    {
        return int.Parse(InputField.text);
    }

    //接触判定
    private void OnContactReceived(Guid fromId, Guid toId)
    {
        // 自分が関係ない接触は無視
        if (fromId != myConnectionId && toId != myConnectionId)
            return;

        Debug.Log($"接触発生 from:{fromId} to:{toId}");
    }
}
