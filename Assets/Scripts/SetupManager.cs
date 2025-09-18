using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour {
    // ピースの親オブジェクト
    [SerializeField] public Transform pieceParent;

    // 空白用のピースオブジェクト
    [SerializeField] public PieceManager nullPieceObject;

    // プレイヤーマネージャー
    [SerializeField] private PlayerBattleManager playerManager;

    // ピースを動かしているかどうか
    private bool isMovePiece;
    // ロープを動かしているかどうか
    private bool isMoveRope;

    // クリックしたオブジェクト
    private GameObject hitPiece;

    // ピースマネージャー
    private PieceManager pieceManager;
    // ショップマネージャー
    private ShopManager shopManager;

    // バトルマネージャー
    [SerializeField] public BattleManager battleManager;

    // ロープのRigitBody
    private Rigidbody2D ropeRb;

    // クリックしたときの座標
    private Vector2 mouseClickPoint = Vector2.zero;
    // クリックし続けているときの座標
    private Vector2 mousePoint = Vector2.zero;

    // おける座標リスト
    static public List<Vector2> CanPutPiecePosList { get; private set; }
    // 持っているピース
    static public List<PieceManager> InventoryPieceList { get; private set; }
    // フィールド上にあるピース
    static public PieceManager[] FieldPieceList { get; private set; }

    // セットアップのUIキャンバス
    [SerializeField] public GameObject setupUICanvas;

    // ピースの説明用UI
    [SerializeField] private GameObject pieceInfoPanel;

    // マネーテキスト
    [SerializeField] private Text moneyText;

    // 売却エリアUI
    [SerializeField] private GameObject sellAreaObject;

    // フィールドサイズ
    const int FieldWidth = 7;
    const int FieldHeight = 7;
    const float PieceScale = 0.5f;

    private void Start() {
        shopManager = GetComponent<ShopManager>();

        // 配列にピースのおける座標を格納
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
        // マウス処理
        // 押したとき
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

                    // 説明を表示
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
                                textUI.text = $"<sprite name=damage>ダメージ : {hitPieceInfo.amount}\n<sprite name=energy>必要エネルギー : {hitPieceInfo.energyCost}\n<sprite name=cooltime>クールタイム : {hitPieceInfo.cooltime}\n" + hitPieceInfo.descriptionText;
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

                    // 説明を非表示
                    pieceInfoPanel.SetActive(false);
                }

            }
            else {
                // 説明を非表示
                pieceInfoPanel.SetActive(false);
            }
        }
        // 押しっぱなし
        else if (Input.GetMouseButton(0)) {
            if (isMovePiece) {
                pieceManager.HoldOnClick();

                // ピースの回転処理
                if (Input.GetMouseButtonDown(1)) {
                    pieceManager.Rotate();
                }
            }
            else if (isMoveRope) {
                // スクリーン座標からワールド座標に変換
                mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 移動処理
                float mouseDis = mouseClickPoint.y - mousePoint.y;
                if (mouseDis <= 3.3f) {
                    ropeRb.MovePosition(mousePoint);
                }
            }
        }
        // 離したとき
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

        // 最初のピースとつながっているか
        IsChainedFirstPiece();

        SetListPiece();
    }

    // リロール判定
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

    // マウスポジションに当たり判定があったピースを返す
    private GameObject GetHitPiece() {
        GameObject ret = null;

        // スクリーン座標からワールド座標に変換
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (RaycastHit2D hit2d in Physics2D.RaycastAll(worldPoint, Vector2.zero)) {
            // 当たり判定あり
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

        // [ピースの親オブジェクト]の子要素を[持っているピースリスト]に追加
        foreach (Transform child in pieceParent) {
            InventoryPieceList.Add(child.GetComponent<PieceManager>());
        }
        // フィールド上にあるピースだけのリストに追加
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

    // 最初のピースとつながっているか
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

    // Vector3の比較
    private bool CompareVec3(Vector3 a, Vector3 b, float tolerance = 0.005f) {
        return Vector3.Distance(a, b) < tolerance;
    }

    // 初期ピースの配置
    private void SetFirstPiece() {
        // 初期ピースを配置
        GameObject createdPiece = shopManager.CreatePiece(pieceParent, CanPutPiecePosList[24], Quaternion.identity, true, 0);
        createdPiece.GetComponent<PieceManager>().InitFirstPiece();

        SetListPiece();
    }

    // セットアップに戻る
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

    // タイトルから
    public void TitleInit() {
        enabled = true;
        setupUICanvas.SetActive(true);

        enabled = true;

        UpdateMoney();

        shopManager.SortShop();
        battleManager.SetBattlePlayerCharacter();

        SetFirstPiece();
    }

    // バトルスタートボタン
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

    // マネーテキスト更新
    public void UpdateMoney(int value = 0) {
        playerManager.money += value;

        moneyText.text = playerManager.money.ToString();
    }
}
