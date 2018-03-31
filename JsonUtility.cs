using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

/// <summary>
/// JSON読み込みユーティリティ
/// 参考URL : http://takachan.hatenablog.com/entry/2017/01/18/120000
/// </summary>
public static class JsonUtility
{
    public static string GetFileString(string filePath)
    {
        StreamReader file = new StreamReader(filePath, Encoding.UTF8);
        return file.ReadToEnd();
    }

    public static T Deserialize<T>(string filePath)
    {
        string fileString = GetFileString(filePath);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileString)))
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
	}
}
