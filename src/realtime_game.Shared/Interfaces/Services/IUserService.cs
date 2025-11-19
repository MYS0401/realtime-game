using MagicOnion;
using realtime_game.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realtime_game.Shared.Interfaces.Services
{
    public interface IUserService : IService<IUserService>
    {
        //ユーザーを登録するAPI
        UnaryResult<int> RegistUserAsync(string name);

        //id指定でユーザー情報を所得するAPI
        UnaryResult<User> GetUserAsync(int id);

        //ユーザー一覧を取得するAPIs
        UnaryResult<string[]> ShowUserListAsync();

        //id指定でユーザー名を更新するAPI
        UnaryResult<string> UpdateUserAsync(int id);
    }
}
