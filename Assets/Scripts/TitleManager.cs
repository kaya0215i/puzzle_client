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

    // �L�����N�^�[�I��pUI
    [SerializeField] private GameObject characterSelectUICanvas;

    // �C���v�b�g�pUI
    [SerializeField] private GameObject inputUICanvas;

    // �I�v�V����UI
    [SerializeField] private GameObject optionUICavas;

    // �t�����hUI
    [SerializeField] private GameObject friendUICanvas;

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

    // �t�����h���X�g
    [SerializeField] private GameObject friendList;

    // �t�����h���X�g�e�L�X�g
    [SerializeField] private TextMeshProUGUI friendText;

    // �t�����h���N�G�X�g���X�g
    [SerializeField] private GameObject friendRequestList;

    // �t�����h���N�G�X�g���X�g�e�I�u�W�F�N�g
    [SerializeField] private Transform friendRequestParent;

    // �t�����h���N�G�X�g�v���n�u
    [SerializeField] private GameObject friendRequestPrefab;

    // �v���C���[����
    [SerializeField] private GameObject playerSearch;

    // �v���C���[�����e�L�X�g
    [SerializeField] private UnityEngine.UI.Text playerSearchText;

    // �V�X�e�����b�Z�[�W�e�L�X�g
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
        characterSelectUICanvas.SetActive(true);
    }

    // �Q�[���X�^�[�g�X�^�[�g�{�^��
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

    // �t�����h�{�^��
    public void OnClickFriendButton() {
        titleUICanvas.SetActive(false);
        friendUICanvas.SetActive(true);

        OnClickFriendListButton();
    }
    // �t�����h���X�g�{�^��
    public void OnClickFriendListButton() {
        friendText.text = "";

        friendList.SetActive(true);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(false);

        // �t�����h���擾
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

    // �t�����h���X�g���N�G�X�g���X�g�{�^��
    public void OnClicFriendRequestListButton() {
        foreach(Transform child in friendRequestParent) {
            Destroy(child.gameObject);
        }

        friendList.SetActive(false);
        friendRequestList.SetActive(true);
        playerSearch.SetActive(false);

        // �t�����h���N�G�X�g���擾
        StartCoroutine(NetworkManager.Instance.GetFriendRequestData(
            result => {
                if (result != null) {
                    for (int i = 0; i < result.Name.Count; i++) {
                        GameObject obj= Instantiate(friendRequestPrefab, Vector3.zero, Quaternion.identity, friendRequestParent);

                        // ���O�ƃ����N��ݒ�
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

                        // �t�����h���N�G�X�g���{�^���ݒ�
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

    // �t�����h���N�G�X�g���F�{�^��
    public void OnClickFriendRequestAcceptButton(string userName) {
        StartCoroutine(NetworkManager.Instance.AcceptFriendRequest(
            userName,
            result => {
                if (result) {
                    Debug.Log("�t�����h���N�G�X�g�����F���܂����B");
                }
                else {
                    Debug.Log("�t�����h���N�G�X�g�����F�o���܂���ł����B");
                }
            }));
    }

    // �v���C���[�����{�^��
    public void OnClickPlayerSearch() {
        friendList.SetActive(false);
        friendRequestList.SetActive(false);
        playerSearch.SetActive(true);

        systemMessageText.text = "";
    }

    // �t�����h���N�G�X�g���M�{�^��
    public void OnClickSendFriendRequestButton() {
        if(playerSearchText.text.Length < 4) {
            systemMessageText.text = "<color=red>4�����ȏ�œ��͂��Ă��������B</color>";
        }

        StartCoroutine(NetworkManager.Instance.SendFriendRequest(
            playerSearchText.text,
            result => {
                if (result) {
                    systemMessageText.text = "<color=green>���N�G�X�g�𑗐M���܂����B</color>";
                }
                else {
                    systemMessageText.text = "<color=red>���N�G�X�g�𑗐M�o���܂���ł����B</color>";
                    Debug.Log("���N�G�X�g�𑗐M�o���܂���ł����B");
                }
            }));
    }

    // �I�v�V�����{�^��
    public void OnClickOptionButton() {
        titleUICanvas.SetActive(false);
        optionUICavas.SetActive(true);

        changeNameText.text = "";
        changeNamePlaceholder.text = NetworkManager.Instance.UserName;
    }

    // �N���[�Y�{�^��
    public void OnClickCloceButton(GameObject obj) {
        obj.SetActive(false);
        titleUICanvas.SetActive(true);
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
               OnClickCloceButton(optionUICavas);
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
            characterInfoText.text = "<b><color=red>��m</color></b>\n\n�o�����X�̂Ƃꂽ�X�e�[�^�X";
        }
        else if(currentCharacterIndex == 1) {
            characterInfoText.text = "<b><color=red>���m</color></b>\n\n�̗͂ƃG�l���M�[���������U���ƃX�s�[�h���Ⴂ";
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

                    titleUICanvas.SetActive(false);
                    inputUICanvas.SetActive(true);
                    RegistUser();
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
