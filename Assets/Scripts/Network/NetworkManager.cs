using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour {
    // WebAPI�̐ڑ����ݒ�
#if DEBUG
    const string API_BASE_URL = "http://localhost:8000/api/";
#else
    const string API_BASE_URL = "";
#endif

    private string apiToken; // ������APIToken
    private string userName; // ���͂����z��̎����̃��[�U�[ID
    private int userRankId;
    private int userRankPoint;

    // �v���p�e�B
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

    // �ʐM�p�̊֐�
    // ���[�U�[�o�^����
    public IEnumerator RegistUser(string name, Action<bool> result) {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        RegistUserRequest requestData = new RegistUserRequest();
        requestData.Name = name;
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");
        yield return request.SendWebRequest();
        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            RegistUserResponse response = JsonConvert.DeserializeObject<RegistUserResponse>(resultJson);
            // �t�@�C���Ƀ��[�U�[ID��ۑ�
            this.userName = name;
            this.apiToken = response.APIToken;
            SaveUserData();
            isSuccess = true;
        }
        Debug.Log(request.responseCode);
        Debug.Log(request.result);
        result?.Invoke(isSuccess); // �����ŌĂяo������result�������Ăяo��
    }

    // ���[�U�[���X�V
    public IEnumerator UpdateUser(string name, int rankId, int rankPoint, Action<bool> result) {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserRequest requestData = new UpdateUserRequest();
        requestData.UserName = name;
        requestData.RankId = rankId;
        requestData.RankPoint = rankPoint;
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(
                    API_BASE_URL + "users/update", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
         && request.responseCode == 200) {
            // �ʐM�����������ꍇ�A�v���p�e�B�ɍX�V�������[�U�[����ۑ�
            this.userName = name;
            this.userRankId = rankId;
            this.userRankPoint = rankPoint;
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // �����ŌĂяo������result�������Ăяo��
    }

    // ���[�U�[�����擾
    public IEnumerator GetUserData(Action<bool> result) {
        // ���[�U�[���擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "users/index");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest ();
        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            UserDataResponse response = JsonConvert.DeserializeObject<UserDataResponse>(resultJson);
            this.userName = response.UserName;
            this.userRankId = response.RankId;
            this.userRankPoint = response.RankPoint;
            isSuccess= true;
        }
        result?.Invoke(isSuccess);
    }

    // ���[�U�[�̃t�B�[���h�f�[�^��o�^
    public IEnumerator RegistUserFieldData(PlayerBattleManager playerManager, Action<bool> result) {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
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

        // �T�[�o�[�ɑ��M����I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(userDataRequest);

        // ���M
        UnityWebRequest request = UnityWebRequest.Post(
                    API_BASE_URL + "fields/store", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // �����ŌĂяo������result�������Ăяo��
    }

    // �G�̃t�B�[���h�f�[�^���擾
    public IEnumerator GetEnemyFieldData(PlayerBattleManager playerManager, Action<EnemyFieldDataResponse> result) {
        // �t�B�[���h�f�[�^���擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "fields/" + this.userRankId + "/" + playerManager.currentRound);
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200) {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            EnemyFieldDataResponse response = JsonConvert.DeserializeObject<EnemyFieldDataResponse>(resultJson);
            result?.Invoke(response);
        }
        else {
            // �ʐM�����s�����ꍇ
            Debug.Log("�t�B�[���h�����擾�o���܂���ł���");
            string resultJson = "{\r\n  \"name\": \"BOT\",\r\n  \"character_type\": \"Warrior\",\r\n  \"index\": [\r\n    24,\r\n  ],\r\n  \"item_id\": [\r\n    1,\r\n  ],\r\n  \"piece_id\": [\r\n    5,\r\n  ],\r\n  \"piece_angle\": [\r\n    {\r\n      \"x\": 0,\r\n      \"y\": 0,\r\n      \"z\": 0,\r\n      \"w\": 1\r\n    }\r\n  ]\r\n}";
            EnemyFieldDataResponse response = JsonConvert.DeserializeObject<EnemyFieldDataResponse>(resultJson);
            Debug.Log(response.CharacterType);
            result?.Invoke(response);
        }
    }

    // ���[�U�[Token��ۑ�����
    private void SaveUserData() {
        SaveData saveData = new SaveData();
        saveData.APIToken = this.APIToken;
        string json = JsonConvert.SerializeObject(saveData);
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    // ���[�U�[Token��ǂݍ���
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
