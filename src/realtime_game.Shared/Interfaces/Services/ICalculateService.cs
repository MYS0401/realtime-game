using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;

namespace realtime_game.Shared.Interfaces.Services
{
    /// <summary>
    /// 初めてのRPGサービス
    /// </summary>
    public interface ICalculateService : IService<ICalculateService>
    {
        //[ここにどのようなAPIを作るのか定義を作成する]

        /// <summary>
        /// 乗算処理
        /// </summary>
        /// <param name="x">数1つ目</param>
        /// <param name="y">数2つ目</param>
        /// <returns>XとYの乗算値</returns>
        //『乗算API』二つの整数を引数で受け取り乗算値を返す
        UnaryResult<int> MulAsync(int x,int y);

        //文字を表示
        UnaryResult<string> ShowAsync(string text);

        //受け取った配列の値の合計を出す
        UnaryResult<int> SumAllAsync(int[] numList);

        //X+Yを[0]、X-Yを[1]、X*Yを[2]、X/Yを[3]の入れて配列で返す
        UnaryResult<int[]> CalcForOperationAsync(int x, int y);

        //小数の値3つをフィールドに持つNumberクラスを渡して、3つの値の合計値を返す
        //UnaryResult<float> SumAllNumberAsync(Number numData);
    }
}
