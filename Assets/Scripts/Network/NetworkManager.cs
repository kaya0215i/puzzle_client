using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    // WebAPIの接続先を設定
#if DEBUG
    const string API_BASE_URL = "";
#else
    const string API_BASE_URL = "";
#endif

    private string apiToken; // 自分のAPIToken
    private string userName; // 入力される想定の自分のユーザーID

    // プロパティ
    public string APIToken {
        get { return this.apiToken; }
    }

    public string UserName {
        get { return this.userName; }
    }

    private static NetworkManager instance;
    public static NetworkManager Instance {
        get {
            if (instance == null) {
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    // 通信用の関数
}
