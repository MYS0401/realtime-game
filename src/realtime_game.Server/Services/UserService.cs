using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MessagePack.Formatters;
using realtime_game.Server.Models.Contexts;
using realtime_game.Shared.Models.Entities;
using realtime_game.Shared.Interfaces.Services;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using realtime_game.Server.StreamingHubs;


public class UserService : ServiceBase<IUserService>, IUserService
{
    public async UnaryResult<int> RegistUserAsync(string name)
    {
        using var context = new GameDbContext();
        //バリデーションチェック(名前登録済みかどうか)
        if (context.Users.Count() > 0 &&
              context.Users.Where(user => user.Name == name).Count() > 0)
        {
            throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "");
        }
        //テーブルにレコードを追加
        User user = new User();
        user.Name = name;
        user.Token = "";
        user.Created_at = DateTime.Now;
        user.Updated_at = DateTime.Now;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;

    }

    //id指定でユーザー情報を所得するAPI
    public async UnaryResult<User> GetUserAsync(int id)
    {
        using var context = new GameDbContext();

        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            throw new ReturnStatusException(StatusCode.NotFound, "");
        }

        return new User
        {
            Id = user.Id,
            Name = user.Name,
            Token = user.Token,
            Created_at = user.Created_at,
            Updated_at = user.Updated_at,
        };
    }

    //ユーザー一覧を取得するAPI
    public async UnaryResult<string[]> ShowUserListAsync()
    {
        using var context = new GameDbContext();

        var users = await context
            .Users
            .OrderBy(user => user.Id)
            .Select(user => user.Name)
            .ToArrayAsync();

        return users;
    }

    //id指定でユーザー名を更新するAPI
    public async UnaryResult<string> UpdateUserAsync(int id)
    {
        //
        return "";
    }
}

