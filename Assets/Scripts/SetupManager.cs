using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour {
    // �s�[�X�̐e�I�u�W�F�N�g
    [SerializeField] public Transform pieceParent;

    // �󔒗p�̃s�[�X�I�u�W�F�N�g
    [SerializeField] public PieceManager nullPieceObject;

    // �v���C���[�}�l�[�W���[
    [SerializeField] private PlayerBattleManager playerManager;

    // �s�[�X�𓮂����Ă��邩�ǂ���
    private bool isMovePiece;
    // ���[�v�𓮂����Ă��邩�ǂ���
    private bool isMoveRope;

    // �N���b�N�����I�u�W�F�N�g
    private GameObject hitPiece;

    // �s�[�X�}�l�[�W���[
    private PieceManager pieceManager;
    // �V���b�v�}�l�[�W���[
    private ShopManager shopManager;

    // �o�g���}�l�[�W���[
    [SerializeField] public BattleManager battleManager;

    // ���[�v��RigitBody
    private Rigidbody2D ropeRb;

    // �N���b�N�����Ƃ��̍��W
    private Vector2 mouseClickPoint = Vector2.zero;
    // �N���b�N�������Ă���Ƃ��̍��W
    private Vector2 mousePoint = Vector2.zero;

    // ��������W���X�g
    static public List<Vector2> CanPutPiecePosList { get; private set; }
    // �����Ă���s�[�X
    static public List<PieceManager> InventoryPieceList { get; private set; }
    // �t�B�[���h��ɂ���s�[�X
    static public PieceManager[] FieldPieceList { get; private set; }

    // �Z�b�g�A�b�v��UI�L�����o�X
    [SerializeField] public GameObject setupUICanvas;

    // �s�[�X�̐����pUI
    [SerializeField] private GameObject pieceInfoPanel;

    // �}�l�[�e�L�X�g
    [SerializeField] private Text moneyText;

    // ���p�G���AUI
    [SerializeField] private GameObject sellAreaObject;

    // �t�B�[���h�T�C�Y
    const int FieldWidth = 7;
    const int FieldHeight = 7;
    const float PieceScale = 0.5f;

    private void Start() {
        shopManager = GetComponent<ShopManager>();

        // �z��Ƀs�[�X�̂�������W���i�[
        CanPutPiecePosList = new List<Vector2>();
        for (int i = 0;  i < FieldHeight; i++) {
            for (int j = 0; j < FieldWidth; j++) {
                CanPutPiecePosList.Add(new Vector2(-1.5f + (j * PieceScale), -3.5f + (i * PieceScale)));
            }
        }

        isMovePiece = false;
        isMoveRope = false;

        pieceInfoPanel.SetActive(false);
        setupUICanvas.SetActive(false);
        sellAreaObject.SetActive(false);

        enabled = false;
    }

    private void Update() {
        // �}�E�X����
        // �������Ƃ�
        if (Input.GetMouseButtonDown(0)) {
            hitPiece = GetHitPiece();

            if (hitPiece != null) {
                if (hitPiece.CompareTag("Piece")) {
                    isMovePiece = true;
                    pieceManager = hitPiece.GetComponent<PieceManager>();
                    pieceManager.OnClick();

                    if (!pieceManager.inShop && !pieceManager.firstPiece) {
                        sellAreaObject.SetActive(true);
                    }

                    // ������\��
                    PieceInfo hitPieceInfo = hitPiece.transform.GetChild(0).GetComponent<PieceInfo>();
                    bool isAttack = hitPieceInfo.isWeapon;

                    pieceInfoPanel.SetActive(true);

                    foreach (Transform child in pieceInfoPanel.transform) {
                        child.gameObject.SetActive(false);

                        TextMeshProUGUI textUI = child.GetComponent<TextMeshProUGUI>();

                        if (child.gameObject.name == "PieceNameText") {
                            child.gameObject.SetActive(true);
                            textUI.text = hitPieceInfo.name;
                        }
                        else if (child.gameObject.name == "PieceItemDescriptionText") {
                            if (isAttack) {
                                child.gameObject.SetActive(true);
                                textUI.text = $"<sprite name=damage>�_���[�W : {hitPieceInfo.amount}\n<sprite name=energy>�K�v�G�l���M�[ : {hitPieceInfo.energyCost}\n<sprite name=cooltime>�N�[���^�C�� : {hitPieceInfo.cooltime}\n" + hitPieceInfo.descriptionText;
                            }
                            else if (!isAttack) {
                                child.gameObject.SetActive(true);
                                textUI.text = hitPieceInfo.descriptionText;
                            }
                        }
                    }

                }
                else {
                    if (hitPiece.CompareTag("Rope")) {
                        isMoveRope = true;
                        mouseClickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        ropeRb = hitPiece.GetComponent<Rigidbody2D>();
                    }

                    // �������\��
                    pieceInfoPanel.SetActive(false);
                }

            }
            else {
                // �������\��
                pieceInfoPanel.SetActive(false);
            }
        }
        // �������ςȂ�
        else if (Input.GetMouseButton(0)) {
            if (isMovePiece) {
                pieceManager.HoldOnClick();

                // �s�[�X�̉�]����
                if (Input.GetMouseButtonDown(1)) {
                    pieceManager.Rotate();
                }
            }
            else if (isMoveRope) {
                // �X�N���[�����W���烏�[���h���W�ɕϊ�
                mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // �ړ�����
                float mouseDis = mouseClickPoint.y - mousePoint.y;
                if (mouseDis <= 3.3f) {
                    ropeRb.MovePosition(mousePoint);
                }
            }
        }
        // �������Ƃ�
        else if (Input.GetMouseButtonUp(0)) {
            isMovePiece = false;
            isMoveRope = false;
            sellAreaObject.SetActive(false);

            if (hitPiece != null) {
                if (hitPiece.CompareTag("Piece")) {
                    pieceManager.ReleaseClick();
                }
                else if (hitPiece.CompareTag("Rope")) {
                    float mouseDis = mouseClickPoint.y - mousePoint.y;
                    if (mouseDis > 1.3f) {
                        if (RerollJudgment()) {
                            shopManager.SortShop();
                        }
                    }
                }
            }
        }

        // �ŏ��̃s�[�X�ƂȂ����Ă��邩
        IsChainedFirstPiece();

        SetListPiece();
    }

    // �����[������
    private bool RerollJudgment() {
        bool isReroll = false;
        int tmp = 1 + (int)(playerManager.rerollCount * 0.2f);

        if (playerManager.money >= tmp) {
            playerManager.rerollCount++;
            isReroll = true ;
            UpdateMoney(-tmp);
        }

        return isReroll;
    }

    // �}�E�X�|�W�V�����ɓ����蔻�肪�������s�[�X��Ԃ�
    private GameObject GetHitPiece() {
        GameObject ret = null;

        // �X�N���[�����W���烏�[���h���W�ɕϊ�
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (RaycastHit2D hit2d in Physics2D.RaycastAll(worldPoint, Vector2.zero)) {
            // �����蔻�肠��
            if (hit2d) {
                //Debug.Log(hit2d.collider.gameObject.name);

                if (hit2d.collider.gameObject.CompareTag("Piece")) {
                    ret = hit2d.collider.gameObject;
                    break;
                }
                if (hit2d.collider.gameObject.CompareTag("Rope")) {
                    ret = hit2d.collider.gameObject;
                    break;
                }
            }
        }
        return ret;
    }

    
    public void SetListPiece() {
        InventoryPieceList = new List<PieceManager>();
        FieldPieceList = new PieceManager[FieldWidth * FieldHeight];

        // [�s�[�X�̐e�I�u�W�F�N�g]�̎q�v�f��[�����Ă���s�[�X���X�g]�ɒǉ�
        foreach (Transform child in pieceParent) {
            InventoryPieceList.Add(child.GetComponent<PieceManager>());
        }
        // �t�B�[���h��ɂ���s�[�X�����̃��X�g�ɒǉ�
        for (int i = 0; i < FieldPieceList.Length; i++) {
            for (int j = 0; j < InventoryPieceList.Count; j++) {
                if (CompareVec3(CanPutPiecePosList[i], (Vector2)InventoryPieceList[j].transform.position) &&
                    (InventoryPieceList[j].isSet)) {
                    FieldPieceList[i] = InventoryPieceList[j];
                    FieldPieceList[i].indexNum = i;
                    break;
                }
                else if (j == InventoryPieceList.Count - 1) {
                    FieldPieceList[i] = nullPieceObject;
                    FieldPieceList[i].indexNum = -1;
                }
            }
        }
    }

    // �ŏ��̃s�[�X�ƂȂ����Ă��邩
    private void IsChainedFirstPiece() {
        foreach (PieceManager pieceManager in FieldPieceList) {
            if (!pieceManager.firstPiece) {
                pieceManager.isChainedFirstPiece = false;
            }
        }

        for (int i = 0; i < 49; i++) {
            foreach (PieceManager pieceManager in FieldPieceList) {
                pieceManager.ChainPieceJudgment();
            }
        }

        foreach (PieceManager pieceManager in FieldPieceList) {
            pieceManager.ChainPieceJudgmentResult();
        }
    }

    // Vector3�̔�r
    private bool CompareVec3(Vector3 a, Vector3 b, float tolerance = 0.005f) {
        return Vector3.Distance(a, b) < tolerance;
    }

    // �����s�[�X�̔z�u
    private void SetFirstPiece() {
        // �����s�[�X��z�u
        GameObject createdPiece = shopManager.CreatePiece(pieceParent, CanPutPiecePosList[24], Quaternion.identity, true, 0);
        createdPiece.GetComponent<PieceManager>().InitFirstPiece();

        SetListPiece();
    }

    // �Z�b�g�A�b�v�ɖ߂�
    public void Init() {
        shopManager.SortShop();

        enabled = true;
        setupUICanvas.SetActive(true);
        foreach (PieceManager pieceManager in SetupManager.FieldPieceList) {
            pieceManager.enabled = true;
        }

        UpdateMoney();
        playerManager.currentRound++;
    }

    // �^�C�g������
    public void TitleInit() {
        enabled = true;
        setupUICanvas.SetActive(true);

        enabled = true;

        UpdateMoney();

        shopManager.SortShop();
        battleManager.SetBattlePlayerCharacter();

        SetFirstPiece();
    }

    // �o�g���X�^�[�g�{�^��
    public void OnStartBattleButton() {
        foreach(PieceManager piece in FieldPieceList) {
            piece.pieceAngle = piece.transform.rotation;
        }

        enabled = false;

        setupUICanvas.SetActive(false);

        foreach (PieceManager pieceManager in SetupManager.FieldPieceList) {
            pieceManager.enabled = false;
        }

        Camera.main.transform.position = new Vector3(15, 0, -10);

        StartCoroutine(battleManager.Init());   
    }

    // �}�l�[�e�L�X�g�X�V
    public void UpdateMoney(int value = 0) {
        playerManager.money += value;

        moneyText.text = playerManager.money.ToString();
    }
}
