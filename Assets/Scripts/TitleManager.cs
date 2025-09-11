using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;

public class TitleManager : MonoBehaviour {
    // �Z�b�g�A�b�v�}�l�[�W���[
    [SerializeField] private SetupManager setupManager;

    // �v���C���[�}�l�[�W���[
    [SerializeField] private PlayerManager playerManager;

    // UI
    [SerializeField] private GameObject titleUICanvas;

    // �L�����N�^�[�̉摜�I�u�W�F�N�g
    [SerializeField] private GameObject[] characterObjects;

    // ���ݑI�𒆂̃L�����N�^�[�I�u�W�F�N�g�̃C���f�b�N�X
    private int currentCharacterIndex;

    // �L�����N�^�[���\���e�L�X�g
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

    // �L�����N�^�[�ύX�{�^����
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

    // �L�����N�^�[�ύX�{�^���E
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

    // �X�^�[�g�{�^��
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

    // �I�v�V�����{�^��
    public void OnClickOptionButton() {
        titleUICanvas.SetActive(false);
    }

    // ��߂�{�^��
    public void OnClickQuitButton() {
        Application.Quit();
    }

    private void UpdateCharacterInfoText() {
        if (currentCharacterIndex == 0) {
            characterInfoText.text = "<b>�L�����N�^�[�^�C�v</b>\n<color=red>��m</color>\n\n�o�����X�̂Ƃꂽ�X�e�[�^�X";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b>�L�����N�^�[�^�C�v</b>\n<color=red>���m</color>\n\n�̗͂ƃG�l���M�[���������U���ƃX�s�[�h���Ⴂ";
        }
    }
}
