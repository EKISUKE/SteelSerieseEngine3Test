using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

public class SSE3Client
{
    public static void Start()
    {
        string programDataPath = Environment.GetEnvironmentVariable("PROGRAMDATA");
        // coreProp.jsonを読み込む
        var coreProp = JsonUtility.Deserialize<CoreProp>(programDataPath + "/SteelSeries/SteelSeries Engine 3/coreProps.json");
        // JSONからアドレス取得
        string address = coreProp.Address;
        // 正規表現でアドレスとポートに分解
        Match match = Regex.Match(address, "^(?<IPAddress>.*):(?<Port>\\d+)$");
        // 取得できなかった場合は処理終了
        if (!match.Success) {
            Console.WriteLine("coreProp.jsonの読み込みに失敗しました。");
            return;
        }
        //サーバーのIPアドレス（または、ホスト名）とポート番号
        string ipOrHost = match.Groups["IPAddress"].ToString();
        string port     = match.Groups["Port"].ToString();
        // sse3接続先を初期化
        _sse3Address    = "http://" + ipOrHost + ":" + port;

        // Httpクライアントでゲーム登録
        // ゲームを登録
        string addGame = JsonUtility.GetFileString("./Resource/gameSetting.json");
        var response   = PostJsonData(addGame, "/game_metadata");
        WaitResponse(response);
        Console.WriteLine("ゲーム登録結果 : " + response.Result);

        // ゲームイベントを登録
        string bindGameEvent = JsonUtility.GetFileString("./Resource/gameEvent.json");
        response = PostJsonData(bindGameEvent, "/bind_game_event");
        WaitResponse(response);
        Console.WriteLine("ゲームイベント登録結果 : " + response.Result);

        // タイマー作成
        Timer timer = new System.Timers.Timer();
        timer.Interval = 5.0f * 1000.0f;
        timer.Elapsed += (sender, e) =>
                        {
                            string gameExec = JsonUtility.GetFileString("./Resource/gameEventExec.json");
                            response = PostJsonData(gameExec, "/game_event");
                            WaitResponse(response);
                            Console.WriteLine("イベント実行 : " + response.Result);
                        };
        timer.Start();
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        timer.Stop();
        // ゲームイベント削除
        string deleteGameEvent = JsonUtility.GetFileString("./Resource/deleteGameEvent.json");
        response = PostJsonData(bindGameEvent, "/remove_game_event");
        WaitResponse(response);
        Console.WriteLine("ゲームイベント削除結果 : " + response.Result);

        // ゲームの削除
        string deleteGame = JsonUtility.GetFileString("./Resource/deleteGame.json");
        response = PostJsonData(bindGameEvent, "/remove_game");
        WaitResponse(response);
        Console.WriteLine("ゲームの削除 : " + response.Result);


        //閉じる
        Console.WriteLine("処理終了");
    }

    public static async Task<string> PostJsonData(string jsonData, string extraAddress)
    {
        Uri uri = new Uri(_sse3Address + extraAddress);
        var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var response    = await _client.PostAsync(uri, jsonContent);
        return await response.Content.ReadAsStringAsync();
    }

    public static void WaitResponse(Task<string> response)
    {
        while (response.Status != TaskStatus.RanToCompletion)
        {
        };
    }

    private static string     _sse3Address = "";
    private static HttpClient _client = new HttpClient(); // http通信用クライアント
}
