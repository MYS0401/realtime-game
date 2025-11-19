using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MessagePack.Formatters;
using realtime_game.Shared.Interfaces.Services;
using static System.Net.Mime.MediaTypeNames;

public class CalculateService : ServiceBase<ICalculateService>,ICalculateService
{
    //『乗算API』二つの整数を引数で受け取り乗算値を返す
    public async UnaryResult<int> MulAsync(int x, int y)
    {
        Console.WriteLine("Received:" + x + "," + y);
        return x * y;
    }

    //文字を表示
    public async UnaryResult<string> ShowAsync(string text)
    {
        Console.WriteLine("Received:" + text);
        return text;
    }

    //受け取った配列の値の合計を出す
    public UnaryResult<int> SumAllAsync(int[] numList)
    {
        int sum = 0;
        for (int i = 0; i < numList.Length; i++)
        {
            sum += numList[i];
        }
        return new UnaryResult<int>(sum);
    }

    //X+Yを[0]、X-Yを[1]、X*Yを[2]、X/Yを[3]の入れて配列で返す
    public UnaryResult<int[]> CalcForOperationAsync(int x, int y)
    {
        int[] result = new int[4];
        result[0] = x + y;
        result[1] = x - y;
        result[2] = x * y;
        result[3] = x / y;
        return new UnaryResult<int[]>(result);
    }
    /*
    //小数の値3つをフィールドに持つNumberクラスを渡して、3つの値の合計値を返す
    public UnaryResult<float> SumAllNumberAsync(Number numData)
    {
        Console.WriteLine();
        return;
    }*/
}
