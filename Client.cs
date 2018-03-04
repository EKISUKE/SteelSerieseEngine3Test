using System;

// TCP通信クライアント
public class TCPClient
{

    public static void Start()
    {
        //サーバーのIPアドレス（または、ホスト名）とポート番号
        string ipOrHost = "127.0.0.1";
        //string ipOrHost = "localhost";
        int port = 49738;
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
        const string addGame = "{\"game\": \"TEST_GAME\", \"game_display_name\": \"My Testing game\", \"icon_color_id\": 5 }";
        TCPClient client = new TCPClient();
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
