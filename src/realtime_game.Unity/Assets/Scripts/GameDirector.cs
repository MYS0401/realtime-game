using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    //[SerializeField] RoomModel roomModel;
    [SerializeField] InputField InputField;
    bool connect1 = false;
    bool connect2 = false;
    RoomModel roomModel;
    UserModel userModel;

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    int myUserId; //自分のユーザーID
    User myself; //自分のユーザー情報を保持

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.OnLeavedUser += this.OnLeavedUser;

        //接続
        await roomModel.ConnectAsync();
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

            //退出
            await roomModel.LeaveAsync("sampleRoom", myUserId);

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

    public  void InputText()
    {
        connect1 = true;
    }

    public int InputUserId()
    {
        return int.Parse(InputField.text);
    }


}
