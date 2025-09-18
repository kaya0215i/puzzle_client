using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CharacterManager;
using static System.Net.Mime.MediaTypeNames;

public class TitleManager : MonoBehaviour {
    // セットアップマネージャー
    [SerializeField] private SetupManager setupManager;

    // プレイヤーマネージャー
    [SerializeField] private PlayerBattleManager playerManager;

    // UI
    [SerializeField] private GameObject titleUICanvas;

    // キャラクター選択用UI
    [SerializeField] private GameObject characterSelectUICanvas;

    // インプット用UI
    [SerializeField] private GameObject inputUICanvas;

    // オプションUI
    [SerializeField] private GameObject optionUICavas;

    // フレンドUI
    [SerializeField] private GameObject friendUICanvas;

    // 名前入力
    [SerializeField] private UnityEngine.UI.Text nameText;

    // 名前変更用プレイスホルダー
    [SerializeField] private UnityEngine.UI.Text changeNamePlaceholder;

    // 名前変更用入力  
    [SerializeField] private UnityEngine.UI.Text changeNameText;

    // キャラクターの画像オブジェクト
    [SerializeField] private GameObject[] characterObjects;

    // 現在選択中のキャラクターオブジェクトのインデックス
    private int currentCharacterIndex;

    // キャラクター情報表示テキスト
    [SerializeField] private UnityEngine.UI.Text characterInfoText;

    // ランク表示テキスト
    [SerializeField] private TextMeshProUGUI rankText;

    // フレンドリスト
    [SerializeField] private GameObject friendList;

    // フレンドリストテキスト
    [SerializeField] private TextMeshProUGUI friendText;

    // フレンドリクエストリスト
    [SerializeField] private GameObject friendRequestList;

    // フレンドリクエストリスト親オブジェクト
    [SerializeField] private Transform friendRequestParent;

    // フレンドリクエストプレハブ
    [SerializeField] private GameObject friendRequestPrefab;

    // プレイヤー検索
    [SerializeField] private GameObject playerSearch;

    // プレイヤー検索テキスト
    [SerializeField] private UnityEngine.UI.Text playerSearchText;

    // システムメッセージテキスト
    [SerializeField] private UnityEngine.UI.Text systemMessageText;

    private void Start() {
        Camera.main.transform.position = new Vector3(-15, 0, -10);
        titleUICanvas.SetActive(false);
        characterSelectUICanvas.SetActive(false);
        inputUICanvas.SetActive(false);
        optionUICavas.SetActive(false);
        friendUICanvas.SetActive(false);
        friendList.SetActive(false);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(false);

        for (int i = 0; i < characterObjects.Length; i++) {
            if(i == 0) {
                characterObjects[i].gameObject.SetActive(true);
                continue;
            }
            characterObjects[i].gameObject.SetActive(false);
        }

        currentCharacterIndex = 0;
        UpdateCharacterInfoText();


        // サーバーと通信
        UserDataComm();


    }

    // キャラクター変更ボタン左
    public void OnClickCharacterChangeBtnLeft() {
        currentCharacterIndex --;

        if(currentCharacterIndex < 0) {
            currentCharacterIndex = characterObjects.Length - 1;
        }

        for (int i = 0; i < characterObjects.Length; i++) {
            if (i == currentCharacterIndex) {
                characterObjects[i].gameObject.SetActive(true);
                continue;
            }
            characterObjects[i].gameObject.SetActive(false);
        }

        UpdateCharacterInfoText();
    }

    // キャラクター変更ボタン右
    public void OnClickCharacterChangeBtnRight() {
        currentCharacterIndex++;

        if (currentCharacterIndex > characterObjects.Length - 1) {
            currentCharacterIndex = 0;
        }

        for (int i = 0; i < characterObjects.Length; i++) {
            if (i == currentCharacterIndex) {
                characterObjects[i].gameObject.SetActive(true);
                continue;
            }
            characterObjects[i].gameObject.SetActive(false);
        }

        UpdateCharacterInfoText();
    }

    // スタートボタン
    public void OnClickStartButton() {
        titleUICanvas.SetActive(false);
        characterSelectUICanvas.SetActive(true);
    }

    // ゲームスタートスタートボタン
    public void OnClickGameStartButton() {
        characterSelectUICanvas.SetActive(false);

        switch (currentCharacterIndex) {
            case 0:
                playerManager.characterType = CHARACTER_TYPE.Warrior;
                break;

            case 1:
                playerManager.characterType = CHARACTER_TYPE.Tank;
                break;
        }

        setupManager.TitleInit();
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    // フレンドボタン
    public void OnClickFriendButton() {
        titleUICanvas.SetActive(false);
        friendUICanvas.SetActive(true);

        OnClickFriendListButton();
    }
    // フレンドリストボタン
    public void OnClickFriendListButton() {
        friendText.text = "";

        friendList.SetActive(true);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(false);

        // フレンドを取得
        StartCoroutine(NetworkManager.Instance.GetFriendData(
            result => {
                if (result != null) {
                    for (int i = 0; i < result.Name.Count; i++) {
                        switch (result.RankId[i]) {
                            case 1:
                                friendText.text += "<sprite name=iron> ";
                                break;
                            case 2:
                                friendText.text += "<sprite name=bronze> ";
                                break;
                            case 3:
                                friendText.text += "<sprite name=silver> ";
                                break;
                            case 4:
                                friendText.text += "<sprite name=gold> ";
                                break;
                            case 5:
                                friendText.text += "<sprite name=platinum> ";
                                break;
                            case 6:
                                friendText.text += "<sprite name=diamond> ";
                                break;
                            case 7:
                                friendText.text += "<sprite name=legend> ";
                                break;
                        }

                        friendText.text += result.RankPoint[i] + " : ";
                        friendText.text += result.Name[i] + "\n";
                    }
                }
            }));
    }

    // フレンドリストリクエストリストボタン
    public void OnClicFriendRequestListButton() {
        foreach(Transform child in friendRequestParent) {
            Destroy(child.gameObject);
        }

        friendList.SetActive(false);
        friendRequestList.SetActive(true);
        playerSearch.SetActive(false);

        // フレンドリクエストを取得
        StartCoroutine(NetworkManager.Instance.GetFriendRequestData(
            result => {
                if (result != null) {
                    for (int i = 0; i < result.Name.Count; i++) {
                        GameObject obj= Instantiate(friendRequestPrefab, Vector3.zero, Quaternion.identity, friendRequestParent);

                        // 名前とランクを設定
                        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

                        switch (result.RankId[i]) {
                            case 1:
                                text.text += "<sprite name=iron> ";
                                break;
                            case 2:
                                text.text += "<sprite name=bronze> ";
                                break;
                            case 3:
                                text.text += "<sprite name=silver> ";
                                break;
                            case 4:
                                text.text += "<sprite name=gold> ";
                                break;
                            case 5:
                                text.text += "<sprite name=platinum> ";
                                break;
                            case 6:
                                text.text += "<sprite name=diamond> ";
                                break;
                            case 7:
                                text.text += "<sprite name=legend> ";
                                break;
                        }

                        text.text += result.RankPoint[i] + " : ";
                        text.text += result.Name[i];
                        Debug.Log(result.Name[i]);

                        // フレンドリクエスト許可ボタン設定
                        int index = i;

                        Button acceptBtn = obj.GetComponentInChildren<Button>();
                        acceptBtn.onClick.AddListener(() => {
                            OnClickFriendRequestAcceptButton(result.Name[index]);
                            Destroy(obj.gameObject);
                        });
                    }
                }
            }));
    }

    // フレンドリクエスト承認ボタン
    public void OnClickFriendRequestAcceptButton(string userName) {
        StartCoroutine(NetworkManager.Instance.AcceptFriendRequest(
            userName,
            result => {
                if (result) {
                    Debug.Log("フレンドリクエストを承認しました。");
                }
                else {
                    Debug.Log("フレンドリクエストを承認出来ませんでした。");
                }
            }));
    }

    // プレイヤー検索ボタン
    public void OnClickPlayerSearch() {
        friendList.SetActive(false);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(true);

        systemMessageText.text = "";
    }

    // フレンドリクエスト送信ボタン
    public void OnClickSendFriendRequestButton() {
        if(playerSearchText.text.Length < 4) {
            systemMessageText.text = "<color=red>4文字以上で入力してください。</color>";
        }

        StartCoroutine(NetworkManager.Instance.SendFriendRequest(
            playerSearchText.text,
            result => {
                if (result) {
                    systemMessageText.text = "<color=green>リクエストを送信しました。</color>";
                }
                else {
                    systemMessageText.text = "<color=red>リクエストを送信出来ませんでした。</color>";
                    Debug.Log("リクエストを送信出来ませんでした。");
                }
            }));
    }

    // オプションボタン
    public void OnClickOptionButton() {
        titleUICanvas.SetActive(false);
        optionUICavas.SetActive(true);

        changeNameText.text = "";
        changeNamePlaceholder.text = NetworkManager.Instance.UserName;
    }

    // クローズボタン
    public void OnClickCloceButton(GameObject obj) {
        obj.SetActive(false);
        titleUICanvas.SetActive(true);
    }

    // 名前変更ボタン
    public void OnClickChangeNameButton() {
        // ユーザーデータを更新して画面も更新
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            changeNameText.text,       // 名前
            NetworkManager.Instance.UserRankId, // ランクID
            NetworkManager.Instance.UserRankPoint, // ランクポイント
       result => {     // 登録終了後の処理
           if (result == true) {
               OnClickCloceButton(optionUICavas);
           }
           else {
               Debug.Log("ユーザー情報更新が正常に終了しませんでした。");
           }
       }));
    }

    // やめるボタン
    public void OnClickQuitButton() {
        UnityEngine.Application.Quit();
    }

    // キャラクター情報更新
    private void UpdateCharacterInfoText() {
        if (currentCharacterIndex == 0) {
            characterInfoText.text = "<b><color=red>戦士</color></b>\n\nバランスのとれたステータス";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b><color=red>守護士</color></b>\n\n体力とエネルギーが多いが攻撃とスピードが低い";
        }
    }

    // サーバーと通信
    public void UserDataComm() {
        bool isSuccess = NetworkManager.Instance.LoadUserData();
        if (isSuccess) {
            titleUICanvas.SetActive(true);
            GetUserData();
        }
        else {
            //ユーザーデータが保存されてない場合は登録
            inputUICanvas.SetActive(true);
        }
    }

    // ユーザー登録
    public void RegistUser() {
        if(nameText.text.Length > 4) {
            StartCoroutine(NetworkManager.Instance.RegistUser(
               nameText.text,           // 名前
          result => {                          // 登録終了後の処理
              if (result == true) {
                  titleUICanvas.SetActive(true);
                  inputUICanvas.SetActive(false);
                  GetUserData();
              }
              else {
                  Debug.Log("ユーザー登録が正常に終了しませんでした。");
              }
          }));
        }
    }

    // ユーザー情報取得
    private void GetUserData() {
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result => {
                if(result == true) {
                    Debug.Log("ユーザー名 : " + NetworkManager.Instance.UserName +
                        "\nユーザーランク : " + NetworkManager.Instance.UserRankId + 
                        "\nユーザーランクポイント : " + NetworkManager.Instance.UserRankPoint);

                    playerManager.SetName(NetworkManager.Instance.UserName);
                    UpdateRankText();
                }
                else {
                    Debug.Log("ユーザーを取得出来ませんでした");

                    titleUICanvas.SetActive(false);
                    inputUICanvas.SetActive(true);
                    RegistUser();
                }
            }
        ));
    }

    // ランクテキストを更新
    private void UpdateRankText() {
        int rank = NetworkManager.Instance.UserRankId;
        int rankPoint = NetworkManager.Instance.UserRankPoint;

        rankText.text = "";
        switch (rank) {
            case 1:
                rankText.text = "<sprite name=iron> ";
                break;
            case 2:
                rankText.text = "<sprite name=bronze> ";
                break;
            case 3:
                rankText.text = "<sprite name=silver> ";
                break;
            case 4:
                rankText.text = "<sprite name=gold> ";
                break;
            case 5:
                rankText.text = "<sprite name=platinum> ";
                break;
            case 6:
                rankText.text = "<sprite name=diamond> ";
                break;
            case 7:
                rankText.text = "<sprite name=legend> ";
                break;
        }

        rankText.text += rankPoint;
    }
}
