using System;
using System.Text.RegularExpressions;

// TCP通信クライアント
public class TCPClient
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
        if (!match.Success) return;
        //サーバーのIPアドレス（または、ホスト名）とポート番号
        string ipOrHost = match.Groups["IPAddress"].ToString();
        int port = int.Parse(match.Groups["Port"].ToString());
        Console.WriteLine("{0}:{1}へアクセスします。", ipOrHost, port);
        //TcpClientを作成し、サーバーと接続する
        System.Net.Sockets.TcpClient tcp =
            new System.Net.Sockets.TcpClient(ipOrHost, port);
        Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);

        //NetworkStreamを取得する
        System.Net.Sockets.NetworkStream ns = tcp.GetStream();

        //読み取り、書き込みのタイムアウトを10秒にする
        //デフォルトはInfiniteで、タイムアウトしない
        //(.NET Framework 2.0以上が必要)
        ns.ReadTimeout = 10000;
        ns.WriteTimeout = 10000;

        // ゲームを登録
        TCPClient   client  = new TCPClient();
        string      addGame = JsonUtility.GetFileString("./Resource/gameSetting.json");
        client.SendMessage(addGame, ns);    

        do {

        }while(true);

        // //閉じる
        // ns.Close();
        // tcp.Close();
        // Console.WriteLine("切断しました。");

        // Console.ReadLine();
    }

    //サーバーにメッセージを送信する
    public void SendMessage(string msg, System.Net.Sockets.NetworkStream ns)
    {
        //文字列をByte型配列に変換
        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        byte[] sendBytes = enc.GetBytes(msg + '\n');
        //データを送信する
        ns.Write(sendBytes, 0, sendBytes.Length);
        Console.WriteLine(msg);
    }
}
