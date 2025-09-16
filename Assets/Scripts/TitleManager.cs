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

    // インプット用UI
    [SerializeField] private GameObject inputUICanvas;

    // オプションUI
    [SerializeField] private GameObject optionUICavas;

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

    private void Start() {
        Camera.main.transform.position = new Vector3(-15, 0, -10);
        titleUICanvas.SetActive(false);
        inputUICanvas.SetActive(false);
        optionUICavas.SetActive(false);

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

    // オプションボタン
    public void OnClickOptionButton() {
        titleUICanvas.SetActive(false);
        optionUICavas.SetActive(true);

        changeNameText.text = "";
        changeNamePlaceholder.text = NetworkManager.Instance.UserName;
    }

    // オプションクローズボタン
    public void OnClickOptionCloseButton() {
        titleUICanvas.SetActive(true);
        optionUICavas.SetActive(false);
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
               OnClickOptionCloseButton();
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
            characterInfoText.text = "<b>キャラクタータイプ</b>\n<color=red>戦士</color>\n\nバランスのとれたステータス";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b>キャラクタータイプ</b>\n<color=red>守護士</color>\n\n体力とエネルギーが多いが攻撃とスピードが低い";
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
