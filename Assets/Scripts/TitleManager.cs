using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;

public class TitleManager : MonoBehaviour {
    // セットアップマネージャー
    [SerializeField] private SetupManager setupManager;

    // プレイヤーマネージャー
    [SerializeField] private PlayerManager playerManager;

    // UI
    [SerializeField] private GameObject titleUICanvas;

    // キャラクターの画像オブジェクト
    [SerializeField] private GameObject[] characterObjects;

    // 現在選択中のキャラクターオブジェクトのインデックス
    private int currentCharacterIndex;

    // キャラクター情報表示テキスト
    [SerializeField] private Text characterInfoText;

    private void Start() {
        Camera.main.transform.position = new Vector3(-15, 0, -10);
        titleUICanvas.SetActive(true);

        for(int i = 0; i < characterObjects.Length; i++) {
            if(i == 0) {
                characterObjects[i].gameObject.SetActive(true);
                continue;
            }
            characterObjects[i].gameObject.SetActive(false);
        }

        currentCharacterIndex = 0;
        UpdateCharacterInfoText();
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
    }

    // やめるボタン
    public void OnClickQuitButton() {
        Application.Quit();
    }

    private void UpdateCharacterInfoText() {
        if (currentCharacterIndex == 0) {
            characterInfoText.text = "<b>キャラクタータイプ</b>\n<color=red>戦士</color>\n\nバランスのとれたステータス";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b>キャラクタータイプ</b>\n<color=red>守護士</color>\n\n体力とエネルギーが多いが攻撃とスピードが低い";
        }
    }
}
