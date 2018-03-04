using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

// coreProp.json読み込み用のクラス
[DataContract]
public class CoreProp
{
    // 接続先アドレス
    [DataMember(Name = "address")]
    public string Address {get; set;}

    [DataMember(Name = "encrypted_address")]
    public string EncryptedAddress{get; set;}
}