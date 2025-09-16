using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class PieceManager : MonoBehaviour {
    // ピースのくぼみ
    // [0] => N, [1], => E, [2] => S, [3] => W
    public bool[] dentDirections;
    public int pieceFormId;
    public int itemNum;
    public Quaternion pieceAngle;

    public int indexNum;
    public bool isChainedFirstPiece;

    private SetupManager setupManager;
    public bool isTouch;
    public bool inField;
    public bool isRotate;
    public bool isSet;
    public bool firstPiece;

    public bool isSell;

    public bool inShop;
    public Vector2 shopPos = Vector2.zero;

    private Rigidbody2D myRb;

    private SpriteRenderer mySpriteRenderer;

    private PieceInfo pieceInfo;

    private PlayerBattleManager playerManager;

    private void Awake() {
        setupManager = GameObject.Find("SetupManager").GetComponent<SetupManager>();

        indexNum = -1;
        isChainedFirstPiece = false;
        isTouch = false;
        inField = false;
        isRotate = false;
        isSet = false;
        firstPiece = false;
        isSell = false;

        myRb = this.gameObject.GetComponent<Rigidbody2D>();
        mySpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start() {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerBattleManager>();
    }

    private void Update() {
        if (isTouch) {
            myRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            myRb.velocity = Vector2.zero;
            indexNum = -1;

            if (!isRotate) {
                AngleAdjustment();
            }
        }
        else {
            myRb.constraints = RigidbodyConstraints2D.None;
        }

        if(isSet || inShop) {
            myRb.bodyType = RigidbodyType2D.Kinematic;
            myRb.velocity = Vector2.zero;
        }
        else {
            myRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void SetPiecePos() {
        if (inField && !inShop) {
            // おける座標に一番近い座標を探してそこに移動
            Vector2 setPos = new Vector2(999, 999);
            float comparePosDis = 999f;

            int index = 0;
            foreach (Vector2 pos in SetupManager.CanPutPiecePosList) {
                float thisPosDis = Vector2.Distance((Vector2)this.gameObject.transform.position, pos);
                comparePosDis = Vector2.Distance((Vector2)this.gameObject.transform.position, setPos);

                bool skip1 = false;
                // すでにおいてあったらスキップ
                foreach (var putted in SetupManager.FieldPieceList) {
                    if (CompareVec3(putted.transform.position, pos) && putted.gameObject.CompareTag("Piece")) {
                        skip1 = true;
                        break;
                    }
                }

                bool skip2 = true;
                // 形がかみ合わなかったらスキップ
                // 左
                if (index % 7 != 0) {
                    if (SetupManager.FieldPieceList[index - 1].gameObject.CompareTag("Piece")) {
                        if (SetupManager.FieldPieceList[index - 1].dentDirections[1] == dentDirections[3]) {
                            skip1 = true;
                        }
                        skip2 = false;
                    }
                }
                // 右
                if ((index + 1) % 7 != 0) {
                    if (SetupManager.FieldPieceList[index + 1].gameObject.CompareTag("Piece")) {
                        if (SetupManager.FieldPieceList[index + 1].dentDirections[3] == dentDirections[1]) {
                            skip1 = true;
                        }
                        skip2 = false;
                    }
                }
                // 上
                if (!((42 <= index) && (index <= 48))) {
                    if (SetupManager.FieldPieceList[index + 7].gameObject.CompareTag("Piece")) {
                        if (SetupManager.FieldPieceList[index + 7].dentDirections[2] == dentDirections[0]) {
                            skip1 = true;
                        }
                        skip2 = false;
                    }
                }
                // 下
                if (!((0 <= index) && (index <= 6))) {
                    if (SetupManager.FieldPieceList[index - 7].gameObject.CompareTag("Piece")) {
                        if (SetupManager.FieldPieceList[index - 7].dentDirections[0] == dentDirections[2]) {
                            skip1 = true;
                        }
                        skip2 = false;
                    }
                }

                index++;

                if (skip1) {
                    continue;
                }

                if (skip2) {
                    continue;
                }

                if (thisPosDis < comparePosDis) {
                    setPos = pos;
                    if(index - 1 == 48) {
                        comparePosDis = Vector2.Distance((Vector2)this.gameObject.transform.position, setPos);
                    }
                }
            }

            if(comparePosDis <= 0.5f) {
                this.transform.position = setPos;
                this.gameObject.layer = 6;
                mySpriteRenderer.sortingLayerName = "SetObject";
                isSet = true;
            }
            else {
                inField = false;
            }
        }
        else if (inShop) {
            this.transform.position = shopPos;
            this.transform.eulerAngles = Vector3.zero;
        }
    }

    // Vector3の比較
    private bool CompareVec3(Vector3 a, Vector3 b, float tolerance = 0.005f) {
        return Vector3.Distance(a, b) < tolerance;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        // おける範囲に入ったら
        if (collision.gameObject.CompareTag("BackField") && isTouch) {
            inField = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        // おける範囲から出たら
        if (collision.gameObject.CompareTag("BackField") && isTouch) {
            inField = false;
        }

        // ショップから出たら
        if (collision.gameObject.CompareTag("BackShop") && isTouch && inShop) {
            pieceInfo = this.transform.GetChild(0).GetComponent<PieceInfo>();
            if (pieceInfo.price > playerManager.money) {
                ReleaseClick();
                return;
            }

            setupManager.UpdateMoney(-pieceInfo.price);

            inShop = false;
            this.gameObject.transform.parent = setupManager.pieceParent;
        }
        else {
            isSell = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // ショップに入ったら
        if(collision.gameObject.CompareTag("BackShop") && isTouch && !inShop) {
            isSell = true;
        }
    }

    // 回転調整
    public void AngleAdjustment() {
        List<float> angles = new List<float>() {
            0, 90, 180, 270
        };


        float rotZ = Functions.Nearest(angles, this.transform.eulerAngles.z);
        this.transform.eulerAngles = new Vector3(0, 0, rotZ);

        // くぼみの調整
        bool[] tmpDentDirections = new bool[4];

        if (pieceFormId == 0) {
            tmpDentDirections = new bool[4] { false, false, false, false };
        }
        else if (pieceFormId == 1) {
            tmpDentDirections = new bool[4] { true, false, false, false };
        }
        else if (pieceFormId == 2) {
            tmpDentDirections = new bool[4] { true, true, false, false };
        }
        else if (pieceFormId == 3) {
            tmpDentDirections = new bool[4] { false, true, false, true };
        }
        else if (pieceFormId == 4) {
            tmpDentDirections = new bool[4] { true, true, false, true };
        }
        else if (pieceFormId == 5) {
            tmpDentDirections = new bool[4] { true, true, true, true };
        }

        if (rotZ == 90) {
            for (int i = 0; i < 3; i++) {
                bool tmp = tmpDentDirections[3];

                tmpDentDirections[3] = tmpDentDirections[2];
                tmpDentDirections[2] = tmpDentDirections[1];
                tmpDentDirections[1] = tmpDentDirections[0];
                tmpDentDirections[0] = tmp;
            }
            
        }
        else if (rotZ == 180) {
            for (int i = 0; i < 2; i++) {
                bool tmp = tmpDentDirections[3];

                tmpDentDirections[3] = tmpDentDirections[2];
                tmpDentDirections[2] = tmpDentDirections[1];
                tmpDentDirections[1] = tmpDentDirections[0];
                tmpDentDirections[0] = tmp;
            }
        }
        else if (rotZ == 270) {
            bool tmp = tmpDentDirections[3];

            tmpDentDirections[3] = tmpDentDirections[2];
            tmpDentDirections[2] = tmpDentDirections[1];
            tmpDentDirections[1] = tmpDentDirections[0];
            tmpDentDirections[0] = tmp;
        }

        dentDirections = tmpDentDirections;
    }

    // マウスでクリックされたら
    public void OnClick() {
        if (firstPiece) {
            return;
        }
        isTouch = true;
        this.gameObject.layer = 0;
        mySpriteRenderer.sortingLayerName = "TouchingObject";
        isSet = false;
        isChainedFirstPiece = false;

    }

    // マウスでおしっぱ
    public void HoldOnClick() {
        if (firstPiece) {
            return;
        }
        // 移動処理
        // スクリーン座標からワールド座標に変換
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 setPos;
        setPos.x = Mathf.Clamp(worldPoint.x, -2.55f, 2.55f);
        setPos.y = Mathf.Clamp(worldPoint.y, -4.7f, 4.7f);
        this.transform.position = setPos;
    }

    // 回転処理
    public void Rotate() {
        if (firstPiece) {
            return;
        }
        if (!isRotate) {
            isRotate = true;

            StartCoroutine(rt());

            bool tmp = dentDirections[3];

            dentDirections[3] = dentDirections[2];
            dentDirections[2] = dentDirections[1];
            dentDirections[1] = dentDirections[0];
            dentDirections[0] = tmp;
        }
    }

    IEnumerator rt() {
        int i = 0;
        while (i < 6) {
            i++;
            this.transform.Rotate(0, 0, -15);
            yield return null;
        }
        isRotate = false;
    }

    // マウスを離したとき
    public void ReleaseClick() {
        if (firstPiece) {
            return;
        }
        isTouch = false;
        SetPiecePos();
        if (mySpriteRenderer.sortingLayerName == "TouchingObject") {
            mySpriteRenderer.sortingLayerName = "Default";
        }
        StopCoroutine(rt());
        isRotate = false;

        if (isSell) {
            if (pieceInfo.price - playerManager.currentRound <= 0) {
                setupManager.UpdateMoney(1);
            }
            else {
                setupManager.UpdateMoney(pieceInfo.price - playerManager.currentRound);
            }

            Destroy(this.gameObject);

            setupManager.SetListPiece();
        }
    }

    // 初期ピース用
    public void InitFirstPiece() {
        firstPiece = true;
        isChainedFirstPiece = true;
        isTouch = false;
        isRotate = false;
        if (mySpriteRenderer.sortingLayerName == "TouchingObject") {
            mySpriteRenderer.sortingLayerName = "Default";
        }

        this.gameObject.layer = 6;
        this.GetComponent<SpriteRenderer>().sortingLayerName = "SetObject";
        isSet = true;
    }

    // 最初のピースからつながっているか
    public void ChainPieceJudgment() {
        
        if (isSet && isChainedFirstPiece && !isTouch && indexNum != -1) {

            // 左
            if (indexNum % 7 != 0) {
                if ((SetupManager.FieldPieceList[indexNum - 1].gameObject.CompareTag("Piece")) &&
                (SetupManager.FieldPieceList[indexNum - 1].isChainedFirstPiece == false)) {
                    SetupManager.FieldPieceList[indexNum - 1].isChainedFirstPiece = true;
                }
            }
            // 右
            if ((indexNum + 1) % 7 != 0) {
                if ((SetupManager.FieldPieceList[indexNum + 1].gameObject.CompareTag("Piece")) &&
                (SetupManager.FieldPieceList[indexNum + 1].isChainedFirstPiece == false)) {
                    SetupManager.FieldPieceList[indexNum + 1].isChainedFirstPiece = true;
                }
            }
            // 上
            if (!((42 <= indexNum) && (indexNum <= 48))) {
                if ((SetupManager.FieldPieceList[indexNum + 7].gameObject.CompareTag("Piece")) &&
                (SetupManager.FieldPieceList[indexNum + 7].isChainedFirstPiece == false)) {
                    SetupManager.FieldPieceList[indexNum + 7].isChainedFirstPiece = true;
                }
            }
            // 下
            if (!((0 <= indexNum) && (indexNum <= 6))) {
                if ((SetupManager.FieldPieceList[indexNum - 7].gameObject.CompareTag("Piece")) &&
                (SetupManager.FieldPieceList[indexNum - 7].isChainedFirstPiece == false)) {
                    SetupManager.FieldPieceList[indexNum - 7].isChainedFirstPiece = true;
                }
            }
        }

        
    }
    // 最初のピースからつながってなかったら
    public void ChainPieceJudgmentResult() {
        if (!isChainedFirstPiece &&
            isSet) {
            isSet = false;
            inField = false;
            gameObject.layer = 0;
        }
    }
}
