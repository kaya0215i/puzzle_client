using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour {
    // WebAPIの接続先を設定
#if DEBUG
    const string API_BASE_URL = "http://localhost:8000/api/";
#else
    const string API_BASE_URL = "";
#endif

    private string apiToken; // 自分のAPIToken
    private string userName; // 入力される想定の自分のユーザーID
    private int userRankId;
    private int userRankPoint;

    // プロパティ
    public string APIToken {
        get { return this.apiToken; }
    }

    public string UserName {
        get { return this.userName; }
    }

    public int UserRankId {
        get { return this.userRankId; }
    }

    public int UserRankPoint {
        get { return this.userRankPoint; }
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
    // ユーザー登録処理
    public IEnumerator RegistUser(string name, Action<bool> result) {
        // サーバーに送信するオブジェクトを作成
        RegistUserRequest requestData = new RegistUserRequest();
        requestData.Name = name;
        // サーバーに送信するオブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");
        yield return request.SendWebRequest();
        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            RegistUserResponse response = JsonConvert.DeserializeObject<RegistUserResponse>(resultJson);
            // ファイルにユーザーIDを保存
            this.userName = name;
            this.apiToken = response.APIToken;
            SaveUserData();
            isSuccess = true;
        }
        Debug.Log(request.responseCode);
        Debug.Log(request.result);
        result?.Invoke(isSuccess); // ここで呼び出し元のresult処理を呼び出す
    }

    // ユーザー情報更新
    public IEnumerator UpdateUser(string name, int rankId, int rankPoint, Action<bool> result) {
        // サーバーに送信するオブジェクトを作成
        UpdateUserRequest requestData = new UpdateUserRequest();
        requestData.UserName = name;
        requestData.RankId = rankId;
        requestData.RankPoint = rankPoint;
        // サーバーに送信するオブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(
                    API_BASE_URL + "users/update", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
         && request.responseCode == 200) {
            // 通信が成功した場合、プロパティに更新したユーザー名を保存
            this.userName = name;
            this.userRankId = rankId;
            this.userRankPoint = rankPoint;
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // ここで呼び出し元のresult処理を呼び出す
    }

    // ユーザー情報を取得
    public IEnumerator GetUserData(Action<bool> result) {
        // ユーザー情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "users/index");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest ();
        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            UserDataResponse response = JsonConvert.DeserializeObject<UserDataResponse>(resultJson);
            this.userName = response.UserName;
            this.userRankId = response.RankId;
            this.userRankPoint = response.RankPoint;
            isSuccess= true;
        }
        result?.Invoke(isSuccess);
    }

    // ユーザーのフィールドデータを登録
    public IEnumerator RegistUserFieldData(PlayerBattleManager playerManager, Action<bool> result) {
        // サーバーに送信するオブジェクトを作成
        UserFieldDataRequest userDataRequest = new UserFieldDataRequest();
        userDataRequest.CharacterType = playerManager.characterType;
        userDataRequest.RankId = this.userRankId;
        userDataRequest.CurrentRound = playerManager.currentRound;
        userDataRequest.IndexNum = new List<int>();
        userDataRequest.ItemNum = new List<int>();
        userDataRequest.PieceFormId = new List<int>();
        userDataRequest.PieceAngle = new List<Quaternion>();

        List<PieceManager> battlePieces = new List<PieceManager>();

        foreach (PieceManager piece in SetupManager.FieldPieceList) {
            if (piece.indexNum != -1) {
                battlePieces.Add(piece);
            }
        }

        foreach (PieceManager battlePiece in battlePieces) {

            userDataRequest.IndexNum.Add(battlePiece.indexNum);

            userDataRequest.ItemNum.Add(battlePiece.itemNum);

            userDataRequest.PieceFormId.Add(battlePiece.pieceFormId);

            userDataRequest.PieceAngle.Add(battlePiece.pieceAngle);
        }

        // サーバーに送信するオブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(userDataRequest);

        // 送信
        UnityWebRequest request = UnityWebRequest.Post(
                    API_BASE_URL + "fields/store", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // ここで呼び出し元のresult処理を呼び出す
    }

    // 敵のフィールドデータを取得
    public IEnumerator GetEnemyFieldData(PlayerBattleManager playerManager, Action<EnemyFieldDataResponse> result) {
        // フィールドデータ情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "fields/" + this.userRankId + "/" + playerManager.currentRound);
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            EnemyFieldDataResponse response = JsonConvert.DeserializeObject<EnemyFieldDataResponse>(resultJson);
            result?.Invoke(response);
        }
        else {
            // 通信が失敗した場合
            Debug.Log("フィールド情報を取得出来ませんでした");
            string resultJson = "{\r\n  \"name\": \"BOT\",\r\n  \"character_type\": \"Warrior\",\r\n  \"index\": [\r\n    24,\r\n  ],\r\n  \"item_id\": [\r\n    1,\r\n  ],\r\n  \"piece_id\": [\r\n    5,\r\n  ],\r\n  \"piece_angle\": [\r\n    {\r\n      \"x\": 0,\r\n      \"y\": 0,\r\n      \"z\": 0,\r\n      \"w\": 1\r\n    }\r\n  ]\r\n}";
            EnemyFieldDataResponse response = JsonConvert.DeserializeObject<EnemyFieldDataResponse>(resultJson);
            result?.Invoke(response);
        }
    }

    // フレンド情報取得
    public IEnumerator GetFriendData(Action<UserFriendResponse> result) {
        // フレンド情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "friends/index");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            UserFriendResponse response = JsonConvert.DeserializeObject<UserFriendResponse>(resultJson);
            result?.Invoke(response);
        }
        else {
            result?.Invoke(null);
        }
    }

    // フレンドリクエストを表示
    public IEnumerator GetFriendRequestData(Action<UserFriendResponse> result) {
        // フレンドリクエスト情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "friends/getArrivedFriendRequests");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            if(resultJson.Length > 2) {
                UserFriendResponse response = JsonConvert.DeserializeObject<UserFriendResponse>(resultJson);
                result?.Invoke(response);
            }
            else {
                result?.Invoke(null);
            }
        }
        else {
            result?.Invoke(null);
        }
    }

    // フレンドリクエストを送信
    public IEnumerator SendFriendRequest(string userName, Action<bool> result) {
        // フレンドリクエスト情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "friends/sendFriendRequest/" + userName);
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            isSuccess = true;
        }

        result?.Invoke(isSuccess);
    }

    // フレンドリクエストを承認
    public IEnumerator AcceptFriendRequest(string userName, Action<bool> result) {
        // フレンドリクエスト情報取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "friends/acceptFriendRequest/" + userName);
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            isSuccess = true;
        }

        result?.Invoke(isSuccess);
    }

    // アイテムデータを取得
    public IEnumerator GetItemData(Action<List<ItemDataResponse>> result) {
        // アイテムデータ取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "items/index");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ItemDataRoot response = JsonConvert.DeserializeObject<ItemDataRoot>(resultJson);
            List<ItemDataResponse> itemDate = response.data;
            result?.Invoke(itemDate);
        }

        result?.Invoke(null);
    }

    // ユーザーTokenを保存する
    private void SaveUserData() {
        SaveData saveData = new SaveData();
        saveData.APIToken = this.APIToken;
        string json = JsonConvert.SerializeObject(saveData);
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    // ユーザーTokenを読み込む
    public bool LoadUserData() {
        if(!File.Exists(Application.persistentDataPath + "/saveData.json")) {
            return false;
        }
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.apiToken = saveData.APIToken;
        return true;
    }
}
