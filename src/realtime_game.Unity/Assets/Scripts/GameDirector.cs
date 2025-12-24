using DG.Tweening;
using realtime_game.Shared.Interfaces.StreamingHubs;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    Dictionary<Guid, string> memberNames = new();

    Dictionary<Guid, MemberUI> memberUIList = new();

    int myUserId; //自分のユーザーID
    User myself; //自分のユーザー情報を保持

    bool justWarped = false;

    Guid myConnectionId;

    [SerializeField] GameObject memberItemPrefab;
    [SerializeField] Transform memberListParent;

    bool isMyReady = false;


    public Text countdownText;//始まるまでのカウントダウン
    Coroutine countdownCoroutine;
    bool isCountingDown = false;

    //制限時間
    public Text timelimitText;
    public int timelimit = 10;

    //UI
    public GameObject Retry;
    public GameObject Join;
    public GameObject Leave;
    public GameObject InputRoomName;
    public GameObject InputId;
    public GameObject Panel;
    public GameObject RoomUsers;
    public GameObject Ready;

    public Text endText;

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

        //準備完了
        roomModel.OnReadyStateChangedReceived += this.OnReadyStateChanged;
        roomModel.OnAllReadyReceived += this.OnAllReady;

        roomModel.OnCountdownStartReceived += StartCountdown;
        roomModel.OnCountdownCancelReceived += CancelCountdown;

        //接続
        await roomModel.ConnectAsync();

        // 自分の ConnectionId を保存
        myConnectionId =  roomModel.ConnectionId;

        endText.gameObject.SetActive(false);
        Retry.SetActive(false);

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
            //var joinedUsers = roomModel.JoinAsync("sampleRoom", myUserId);

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

        var ui = CreateMemberUI(user.UserData.Name);
        memberUIList[user.ConnectionId] = ui;

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

            //memberText.text = "";

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

            //memberNames.Remove(Id);
            //RefreshMemberUI();

            Debug.Log("[OnLeavedUser] Destroyed.");
        }
        else
        {
            Debug.LogWarning($"[OnLeavedUser] No Key: {Id}");
        }

        if (memberUIList.TryGetValue(Id, out var ui))
        {
            Destroy(ui.gameObject);
            memberUIList.Remove(Id);
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

    //参加者一覧
    //void RefreshMemberUI()
    //{
    //    //memberText.text += name;

    //    memberText.text = "";
    //    foreach (var name in memberNames.Values)
    //    {
    //        memberText.text += name + "\n";
    //    }
    //}

    public MemberUI CreateMemberUI(string userName)
    {
        // UIプレハブ生成
        GameObject obj = Instantiate(memberItemPrefab, memberListParent);

        // MemberUI コンポーネント取得
        var ui = obj.GetComponent<MemberUI>();

        // 初期表示
        ui.nameText.text = userName;
        ui.readyText.text = "Not Ready";
        ui.readyText.color = Color.gray;

        return ui;
    }

    //準備完了
    public async void OnClickReady()
    {
        isMyReady = !isMyReady;

        Debug.Log($"READY TOGGLE: {isMyReady}");
        //await roomModel.SetReadyAsync(true);
        // サーバー送信
        await roomModel.SetReadyAsync(isMyReady);

        // 自分のUIは更新
        OnReadyStateChanged(myConnectionId, isMyReady);
    }

    public void OnReadyStateChanged(Guid connectionId, bool isReady)
    {
        if (!memberUIList.ContainsKey(connectionId))
            return;

        var ui = memberUIList[connectionId];
        ui.readyText.text = isReady ? "Ready" : "Not Ready";
        ui.readyText.color = isReady ? Color.green : Color.gray;
    }

    public void OnAllReady()
    {
        //Debug.Log("全員Ready！");
        //StartCoroutine(StartCountdown());

        if (isCountingDown) return;

        Debug.Log("全員Ready！カウントダウン開始");
        StartCoroutine(CountdownCoroutine(5));
    }

    IEnumerator CountdownCoroutine(int seconds)
    {
        isCountingDown = true;

        countdownText.gameObject.SetActive(true);

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        StartGame();
    }

    void StartCountdown(int seconds)
    {
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        countdownCoroutine = StartCoroutine(CountdownCoroutine(seconds));
    }

    void CancelCountdown()
    {
        Debug.Log("カウントダウン中止");

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        // UIを元に戻す
        countdownText.text = "";
    }

    void StartGame()
    {
        Debug.Log("ゲーム開始");
        InputBlocker.isBlocked = false;
        //UIを非表示
        Join.SetActive(false);
        Leave.SetActive(false);
        InputRoomName.SetActive(false);
        InputId.SetActive(false);
        Panel.SetActive(false);
        RoomUsers.SetActive(false);
        Ready.SetActive(false);

        StartCoroutine(TimeLimitCoroutine());
    }

    IEnumerator TimeLimitCoroutine()
    {
        while (timelimit >= 0)
        {
            timelimitText.text = timelimit.ToString();
            yield return new WaitForSeconds(1f);
            timelimit--;
        }

        Retry.SetActive(true);
        timelimitText.gameObject.SetActive(false);
        endText.gameObject.SetActive(true);
        InputBlocker.isBlocked = true; //操作を受け付けなくする
    }

    public void RetryButtom()
    {
        //シーンの再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
