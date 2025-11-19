using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnion;
using realtime_game.Shared.Interfaces.Services;
using UnityEngine;
using realtime_game.Shared.Models.Entities;

public class UserModel : BaseModel
{
    private int userId;  //ìoò^ÉÜÅ[ÉUÅ[ID
    public async UniTask<bool> RegistUserAsync(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        try
        {  //ìoò^ê¨å˜
            userId = await client.RegistUserAsync(name);
            return true;
        }
        catch (RpcException e)
        {  //ìoò^é∏îs
            Debug.Log(e);
            return false;
        }
    }

    public async UniTask<User> GetUserAsync(int id)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {
            User userData = await client.GetUserAsync(id);
            return userData;
        }
        catch (RpcException e)
        {
            Debug.LogError(e);
            return null;   // ÉGÉâÅ[éûÇÕ null Çï‘Ç∑
        }
    }

    

}
