using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static CharacterManager;
using static System.Net.Mime.MediaTypeNames;

public class TitleManager : MonoBehaviour {
    // セットアップマネージャー
    [SerializeField] private SetupManager setupManager;

    // プレイヤーマネージャー
    [SerializeField] private PlayerBattleManager playerManager;

    // データマネージャー
    [SerializeField] private DataManager dataManager;

    // UI
    [SerializeField] private GameObject titleUICanvas;

    // タイトルUI
    [SerializeField] private GameObject titleUIParent;

    // キャラクター選択用UI
    [SerializeField] private GameObject characterSelectUIParent;

    // インプット用UI
    [SerializeField] private GameObject inputUIParent;

    // オプションUI
    [SerializeField] private GameObject optionUIParent;

    // フレンドUI
    [SerializeField] private GameObject friendUIParent;

    // 遊び方UI
    [SerializeField] private GameObject howtoUIParent;

    // 遊び方ページオブジェクト
    [SerializeField] private GameObject[] howtoObjects;

    // 遊び方ページ番号テキスト
    [SerializeField] private UnityEngine.UI.Text howtoNumText;

    // 現在の遊び方ページのインデックス
    private int currentHowtoPageIndex;

    // 名前入力
    [SerializeField] private UnityEngine.UI.Text nameText;

    //名前変更用親
    [SerializeField] private InputField changeNameParent;

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

    // 続きからボタン
    [SerializeField] private UnityEngine.UI.Button continueButton;

    private void Start() {
        Camera.main.transform.position = new Vector3(-15, 0, -10);
        titleUICanvas.SetActive(true);
        characterSelectUIParent.SetActive(false);
        inputUIParent.SetActive(false);
        optionUIParent.SetActive(false);
        friendUIParent.SetActive(false);
        friendList.SetActive(false);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(false);
        howtoUIParent.SetActive(false);

        for (int i = 0; i < characterObjects.Length; i++) {
            if(i == 0) {
                characterObjects[i].gameObject.SetActive(true);
                continue;
            }
            characterObjects[i].gameObject.SetActive(false);
        }

        currentCharacterIndex = 0;
        currentHowtoPageIndex = 0;
        UpdateCharacterInfoText();

        AudioManager.Instance.ChangeBGM("TitleBGM");

        // サーバーと通信
        StartCoroutine(UserDataComm());

        continueButton.gameObject.SetActive(false);
    }

    // キャラクター変更ボタン左
    public void OnClickCharacterChangeBtnLeft() {
        AudioManager.Instance.PlayOneShot("Button");

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
        AudioManager.Instance.PlayOneShot("Button");

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
        AudioManager.Instance.PlayOneShot("Button");

        titleUIParent.SetActive(false);
        characterSelectUIParent.SetActive(true);
    }

    // ゲームスタートスタートボタン
    public void OnClickGameStartButton() {
        AudioManager.Instance.PlayOneShot("Button");

        characterSelectUIParent.SetActive(false);
        titleUICanvas.SetActive(false);

        switch (currentCharacterIndex) {
            case 0:
                playerManager.characterType = CHARACTER_TYPE.Warrior;
                break;

            case 1:
                playerManager.characterType = CHARACTER_TYPE.Tank;
                break;

            case 2:
                playerManager.characterType = CHARACTER_TYPE.Thief;
                break;
        }

        setupManager.TitleInit();

        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    // 続きからボタン
    public void OnClickStartInterruptionDataButton() {
        AudioManager.Instance.PlayOneShot("Button");

        titleUICanvas.SetActive(false);

        setupManager.StartInterruptionData();

        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    // フレンドボタン
    public void OnClickFriendButton() {
        AudioManager.Instance.PlayOneShot("Button");

        titleUIParent.SetActive(false);
        friendUIParent.SetActive(true);

        OnClickFriendListButton();
    }

    // フレンドリストボタン
    public void OnClickFriendListButton() {
        AudioManager.Instance.PlayOneShot("Button");

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

    // フレンドリスト リクエストリストボタン
    public void OnClicFriendRequestListButton() {
        AudioManager.Instance.PlayOneShot("Button");

        foreach (Transform child in friendRequestParent) {
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

                        // フレンドリクエスト許可ボタン設定
                        int index = i;

                        UnityEngine.UI.Button acceptBtn = obj.GetComponentInChildren<UnityEngine.UI.Button>();
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
        AudioManager.Instance.PlayOneShot("Button");

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
        AudioManager.Instance.PlayOneShot("Button");

        friendList.SetActive(false);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(true);

        systemMessageText.text = "";
    }

    // フレンドリクエスト送信ボタン
    public void OnClickSendFriendRequestButton() {
        AudioManager.Instance.PlayOneShot("Button");

        if (playerSearchText.text.Length < 4) {
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
        AudioManager.Instance.PlayOneShot("Button");

        titleUIParent.SetActive(false);
        optionUIParent.SetActive(true);

        changeNameParent.text = "";
        changeNamePlaceholder.text = NetworkManager.Instance.UserName;
    }

    // クローズボタン
    public void OnClickCloceButton(GameObject obj) {
        AudioManager.Instance.PlayOneShot("Button");

        obj.SetActive(false);
        titleUIParent.SetActive(true);
    }

    // 名前変更ボタン
    public void OnClickChangeNameButton() {
        AudioManager.Instance.PlayOneShot("Button");
        if (changeNameText.text.Length >= 4) {

            // ユーザーデータを更新して画面も更新
            StartCoroutine(NetworkManager.Instance.UpdateUser(
            changeNameText.text,       // 名前
            NetworkManager.Instance.UserRankId, // ランクID
            NetworkManager.Instance.UserRankPoint, // ランクポイント
       result => {     // 登録終了後の処理
           if (result == true) {
               OnClickCloceButton(optionUIParent);
               playerManager.name = changeNameText.text;
           }
           else {
               Debug.Log("ユーザー情報更新が正常に終了しませんでした。");
           }
       }));
        }
    }

    // 遊び方ボタン
    public void OnClickHowtoButton() {
        titleUIParent.SetActive(false);
        howtoUIParent.SetActive(true);

        howtoObjects[currentHowtoPageIndex].SetActive(true);
        howtoNumText.text = (currentHowtoPageIndex + 1) + "/3";
    }

    // 遊び方ページ切替左
    public void OnClickHowtoPageChangeLeft() {
        if(currentHowtoPageIndex > 0) {
            currentHowtoPageIndex--;

            foreach(GameObject obj in howtoObjects) {
                obj.SetActive(false);
            }

            howtoObjects[currentHowtoPageIndex].SetActive(true);
            howtoNumText.text = (currentHowtoPageIndex + 1) + "/3";
        }
    }

    // 遊び方ページ切替右
    public void OnClickHowtoPageChangeRight() {
        if (currentHowtoPageIndex < 2) {
            currentHowtoPageIndex++;

            foreach (GameObject obj in howtoObjects) {
                obj.SetActive(false);
            }

            howtoObjects[currentHowtoPageIndex].SetActive(true);
            howtoNumText.text = (currentHowtoPageIndex + 1) + "/3";
        }
    }

    // やめるボタン
    public void OnClickQuitButton() {
        AudioManager.Instance.PlayOneShot("Button");
        UnityEngine.Application.Quit();
    }

    // キャラクター情報更新
    private void UpdateCharacterInfoText() {
        switch (currentCharacterIndex) {
            case 0:
                characterInfoText.text = "<b><color=red>戦士</color></b>\n\nバランスのとれたステータス";
                break;

            case 1:
                characterInfoText.text = "<b><color=red>守護士</color></b>\n\n体力とエネルギーが多いが攻撃とスピードが低い";
                break;

            case 2:
                characterInfoText.text = "<b><color=red>盗賊</color></b>\n\nスピードが早くコインを多くもらえるが体力と攻撃力が低い";
                break;
        }
    }

    // サーバーと通信
    public IEnumerator UserDataComm() {
        // アイテムデータを取得
        ItemDataSO.Instance.itemDataList = new List<ItemData>();
        yield return StartCoroutine(NetworkManager.Instance.GetItemData(
            result => {
                if (result != null) {
                    foreach (ItemDataResponse item in result) {
                        if (item != null) {
                            ItemData itemData = new ItemData() {
                                id = item.id,
                                name = item.name,
                                isWeapon = item.isWeapon,
                                amount = item.amount,
                                energyUp = item.energyUp,
                                energyCost = item.energyCost,
                                cooltime = item.cooltime,
                                descriptionText = item.descriptionText,
                                price = item.price,
                            };
                            
                            ItemDataSO.Instance.itemDataList.Add(itemData);
                            Debug.Log(itemData.id);
                        }
                    }
                }
            }));

        bool isSuccess = NetworkManager.Instance.LoadUserData();
        if (isSuccess) {
            yield return GetUserData();

            // 中断データがあるか
            if (dataManager.ExistsInterruptedData()) {
                continueButton.gameObject.SetActive(true);
            }
        }
        else {
            //ユーザーデータが保存されてない場合は登録
            inputUIParent.SetActive(true);
        }
    }

    // ユーザー登録
    public void RegistUser() {
        AudioManager.Instance.PlayOneShot("Button");

        if (nameText.text.Length >= 4) {
            StartCoroutine(NetworkManager.Instance.RegistUser(
               nameText.text,           // 名前
          result => {                          // 登録終了後の処理
              if (result == true) {
                  GetUserData();
                  dataManager.DeleteInterruptedData();
              }
              else {
                  Debug.Log("ユーザー登録が正常に終了しませんでした。");
              }
          }));
        }
    }

    // ユーザー情報取得
    private Coroutine GetUserData() {
        return StartCoroutine(NetworkManager.Instance.GetUserData(
            result => {
                if(result == true) {
                    Debug.Log("ユーザー名 : " + NetworkManager.Instance.UserName +
                        "\nユーザーランク : " + NetworkManager.Instance.UserRankId + 
                        "\nユーザーランクポイント : " + NetworkManager.Instance.UserRankPoint);

                    playerManager.SetName(NetworkManager.Instance.UserName);
                    UpdateRankText();
                    
                    titleUIParent.SetActive(true);
                    inputUIParent.SetActive(false);
                }
                else {
                    Debug.Log("ユーザーを取得出来ませんでした");

                    titleUIParent.SetActive(false);
                    inputUIParent.SetActive(true);
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

    // BGMボリューム変更
    public void OnvalueChangedBGMValumeSslider(UnityEngine.UI.Slider slider) {
        AudioManager.Instance.ChangeBGMValume(slider.value);
    }

    // SEボリューム変更
    public void OnvalueChangedSEValumeSslider(UnityEngine.UI.Slider slider) {
        AudioManager.Instance.ChangeSEValume(slider.value);
    }
}
