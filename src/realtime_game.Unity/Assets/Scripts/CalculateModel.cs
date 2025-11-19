using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using realtime_game.Shared.Interfaces.Services;
using UnityEngine;

public class CalculateModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:5244";

    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] string text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        int result = await Mul(x,y);
        string result1 = await Show(text);

        int[] array = new int[3];
        array[0] = 100;
        array[1] = 200;
        array[2] = 300;

        int result2 = await SumAll(array);

        int[] result3 = new int[3];


        
        Debug.Log("èÊéZåãâ " + result);
        Debug.Log($"{result1}");
        Debug.Log(result2);
        Debug.Log(result3);
    }

    public async UniTask<int> Mul(int x, int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.MulAsync(x, y);
        return result;
    }

    public async UniTask<string> Show(string text)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.ShowAsync(text);
        return result;
    }

    public async UniTask<int> SumAll(int[] NumList)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.SumAllAsync(NumList);
        return result;
    }

    public async UniTask<int[]> CalcForOperationAsync(int x, int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.CalcForOperationAsync(x,y);
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
