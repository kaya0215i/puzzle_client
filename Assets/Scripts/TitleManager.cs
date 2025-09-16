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
    // �Z�b�g�A�b�v�}�l�[�W���[
    [SerializeField] private SetupManager setupManager;

    // �v���C���[�}�l�[�W���[
    [SerializeField] private PlayerBattleManager playerManager;

    // UI
    [SerializeField] private GameObject titleUICanvas;

    // �C���v�b�g�pUI
    [SerializeField] private GameObject inputUICanvas;

    // �I�v�V����UI
    [SerializeField] private GameObject optionUICavas;

    // ���O����
    [SerializeField] private UnityEngine.UI.Text nameText;

    // ���O�ύX�p�v���C�X�z���_�[
    [SerializeField] private UnityEngine.UI.Text changeNamePlaceholder;

    // ���O�ύX�p����  
    [SerializeField] private UnityEngine.UI.Text changeNameText;

    // �L�����N�^�[�̉摜�I�u�W�F�N�g
    [SerializeField] private GameObject[] characterObjects;

    // ���ݑI�𒆂̃L�����N�^�[�I�u�W�F�N�g�̃C���f�b�N�X
    private int currentCharacterIndex;

    // �L�����N�^�[���\���e�L�X�g
    [SerializeField] private UnityEngine.UI.Text characterInfoText;

    // �����N�\���e�L�X�g
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


        // �T�[�o�[�ƒʐM
        UserDataComm();
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
        optionUICavas.SetActive(true);

        changeNameText.text = "";
        changeNamePlaceholder.text = NetworkManager.Instance.UserName;
    }

    // �I�v�V�����N���[�Y�{�^��
    public void OnClickOptionCloseButton() {
        titleUICanvas.SetActive(true);
        optionUICavas.SetActive(false);
    }

    // ���O�ύX�{�^��
    public void OnClickChangeNameButton() {
        // ���[�U�[�f�[�^���X�V���ĉ�ʂ��X�V
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            changeNameText.text,       // ���O
            NetworkManager.Instance.UserRankId, // �����NID
            NetworkManager.Instance.UserRankPoint, // �����N�|�C���g
       result => {     // �o�^�I����̏���
           if (result == true) {
               OnClickOptionCloseButton();
           }
           else {
               Debug.Log("���[�U�[���X�V������ɏI�����܂���ł����B");
           }
       }));
    }

    // ��߂�{�^��
    public void OnClickQuitButton() {
        UnityEngine.Application.Quit();
    }

    // �L�����N�^�[���X�V
    private void UpdateCharacterInfoText() {
        if (currentCharacterIndex == 0) {
            characterInfoText.text = "<b>�L�����N�^�[�^�C�v</b>\n<color=red>��m</color>\n\n�o�����X�̂Ƃꂽ�X�e�[�^�X";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b>�L�����N�^�[�^�C�v</b>\n<color=red>���m</color>\n\n�̗͂ƃG�l���M�[���������U���ƃX�s�[�h���Ⴂ";
        }
    }

    // �T�[�o�[�ƒʐM
    public void UserDataComm() {
        bool isSuccess = NetworkManager.Instance.LoadUserData();
        if (isSuccess) {
            titleUICanvas.SetActive(true);
            GetUserData();
        }
        else {
            //���[�U�[�f�[�^���ۑ�����ĂȂ��ꍇ�͓o�^
            inputUICanvas.SetActive(true);
        }
    }

    // ���[�U�[�o�^
    public void RegistUser() {
        if(nameText.text.Length > 4) {
            StartCoroutine(NetworkManager.Instance.RegistUser(
               nameText.text,           // ���O
          result => {                          // �o�^�I����̏���
              if (result == true) {
                  titleUICanvas.SetActive(true);
                  inputUICanvas.SetActive(false);
                  GetUserData();
              }
              else {
                  Debug.Log("���[�U�[�o�^������ɏI�����܂���ł����B");
              }
          }));
        }
    }

    // ���[�U�[���擾
    private void GetUserData() {
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result => {
                if(result == true) {
                    Debug.Log("���[�U�[�� : " + NetworkManager.Instance.UserName +
                        "\n���[�U�[�����N : " + NetworkManager.Instance.UserRankId + 
                        "\n���[�U�[�����N�|�C���g : " + NetworkManager.Instance.UserRankPoint);

                    playerManager.SetName(NetworkManager.Instance.UserName);
                    UpdateRankText();
                }
                else {
                    Debug.Log("���[�U�[���擾�o���܂���ł���");
                }
            }
        ));
    }

    // �����N�e�L�X�g���X�V
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
