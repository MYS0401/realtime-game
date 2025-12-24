using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace realtime_game.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// サーバーからクライアントへの通知関連
    /// </summary>
    public interface IRoomHubReceiver
    {
        //クライアントに実装

        //サーバーから呼び出す

        //ユーザーの入室通知
        void OnJoin(JoinedUser user);

        //ユーザーの退出通知
        void OnLeave(Guid Id);

        //ユーザーの移動通知
        void OnMove(Guid connectionId, Vector3 pos , Quaternion quaternion);

        //ユーザーの接触判定
        void OnContact(Guid fromConnectionId, Guid toConnectionId);

        //準備完了通知
        void OnReady(Guid connectionId, bool isReady);


        //全員準備完了通知
        void OnAllReady();

        //カウントダウン開始
        void OnCountdownStart(int seconds);

        //カウントダウン中止
        void OnCountdownCancel();

    }

}
