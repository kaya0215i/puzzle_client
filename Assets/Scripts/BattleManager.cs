using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    // セットアップマネージャー
    [SerializeField] private SetupManager setupManager;

    // プレイヤーマネージャー
    [SerializeField] private PlayerManager playerManager;

    // エネミーマネージャー
    [SerializeField] private EnemyManager enemyManager;

    // ショップマネージャー
    [SerializeField] private ShopManager shopManager;

    // バトルのUIキャンバス
    [SerializeField] private GameObject battleUICanvas;

    // プレイヤーのピースの親オブジェクト
    [SerializeField] private Transform playerPieceParent;

    // 敵のピースの親オブジェクト
    [SerializeField] private Transform enemyPieceParent;

    // 生成したプレイヤーのピースリスト
    private List<BattlePieceManager> playerBattlePieceManager;

    // 生成した敵のピースリスト
    private List<BattlePieceManager> enemyBattlePieceManager;

    // プレイヤーのHpスライダー
    [SerializeField] private Slider playerHpSlider;

    // プレイヤーのEnergyスライダー
    [SerializeField] private Slider playerEnergySlider;

    // プレイヤーのHpテキスト
    [SerializeField] private Text playerHpText;

    // プレイヤーのEnergyテキスト
    [SerializeField] private Text playerEnergyText;

    // 敵のHpテキスト
    [SerializeField] private Text enemyHpText;

    // 敵のEnergyテキスト
    [SerializeField] private Text enemyEnergyText;

    // 敵のHpスライダー
    [SerializeField] private Slider enemyHpSlider;

    // 敵のEnergyスライダー
    [SerializeField] private Slider enemyEnergySlider;

    // 戦闘ログ表示トグル
    [SerializeField] private Toggle battleLogToggle;

    // 戦闘ログスクロールビュー
    [SerializeField] private GameObject battleLog;

    // 戦闘ログテキスト
    [SerializeField] private TextMeshProUGUI battleLogText;

    // 戦闘ログScrollRect
    [SerializeField] private ScrollRect scrollRect;

    // Timeテキスト
    [SerializeField] private Text timeText;

    // プレイヤーのキャラオブジェクト
    [SerializeField] private GameObject[] playerCharacters;

    // 敵のキャラオブジェクト
    [SerializeField] private GameObject[] enemyCharacters;

    // VSパネルオブジェクト
    [SerializeField] private GameObject vsPanel;

    // VSパネル用のプレイヤー名テキスト
    [SerializeField] private Text playerVSNameText;

    // VSパネル用のエネミー名テキスト
    [SerializeField] private Text enemyVSNameText;

    // プレイヤーのVSパネルキャラオブジェクト
    [SerializeField] private GameObject[] playerVSCharacters;

    // 敵のVSパネルキャラオブジェクト
    [SerializeField] private GameObject[] enemyVSCharacters;

    // リザルト用パネル
    [SerializeField] private GameObject battleResultPanel;

    // 勝利用オブジェクト
    [SerializeField] private GameObject victoryObject;

    // 敗北用オブジェクト
    [SerializeField] private GameObject defeatObject;

    // 勝利回数表示トロフィー
    [SerializeField] private GameObject[] winTrophys;

    // ライフ表示
    [SerializeField] private GameObject[] playerLifes;

    // ゲームセットパネル
    [SerializeField] private GameObject gameSetPanel;

    // ゲームクリアオブジェクト
    [SerializeField] private GameObject gameClearObject;

    // ゲームオーバーオブジェクト
    [SerializeField] private GameObject gameOverObject;

    // バトルが始まっているか
    private bool isBattleStart;

    // バトルが始まってからの秒数
    private float time;

    // ゲームクリア
    private bool isGameClear;

    // ゲームオーバー
    private bool isGameOver;

    private void Start() {
        battleUICanvas.SetActive(false);
        battleResultPanel.SetActive(false);
        enabled = false;

        isBattleStart = false;

        isGameClear = false;
        isGameOver = false;
    }

    // バトル初期化
    public void Init() {
        enabled = true;
        battleUICanvas.SetActive(true);
        battleResultPanel.SetActive(false);
        gameSetPanel.SetActive(false);

        victoryObject.SetActive(false);
        defeatObject.SetActive(false);
        
        gameClearObject.SetActive(false);
        gameOverObject.SetActive(false);

        time = 0;
        battleLogText.text = "";

        playerManager.StatusReset();
        enemyManager.StatusReset();

        // プレイヤーのデータをサーバーに送信
        UserDataRequest userDataRequest = new UserDataRequest();
        userDataRequest.Name = playerManager.name;
        userDataRequest.CharacterType = playerManager.characterType;
        userDataRequest.currentRound = playerManager.currentRound;
        userDataRequest.IndexNum = new int[49];
        userDataRequest.WeaponAndItemNum = new int[49];
        userDataRequest.PieceFormId = new int[49];
        userDataRequest.PieceAngle = new Quaternion[49];

        for (int i = 0;  i < 49; i++) {

            userDataRequest.IndexNum[i] = new int();
            userDataRequest.WeaponAndItemNum[i] = new int();
            userDataRequest.PieceFormId[i] = new int();
            userDataRequest.PieceAngle[i] = new Quaternion();

            if (!SetupManager.FieldPieceList[i].CompareTag("Piece")) {
                userDataRequest.IndexNum[i]= -1;
                continue;
            }

            userDataRequest.IndexNum[i] = SetupManager.FieldPieceList[i].indexNum;

            userDataRequest.WeaponAndItemNum[i] = SetupManager.FieldPieceList[i].weaponAndItemNum;

            userDataRequest.PieceFormId[i] = SetupManager.FieldPieceList[i].pieceFormId;

            userDataRequest.PieceAngle[i] = SetupManager.FieldPieceList[i].pieceAngle;
        }

        string json = JsonConvert.SerializeObject(userDataRequest);

        Debug.Log(json);

        // プレイヤーのピースを生成
        foreach (Transform child in playerPieceParent) {
            Destroy(child.gameObject);
        }

        playerBattlePieceManager = new List<BattlePieceManager>();

        foreach (PieceManager piece in SetupManager.FieldPieceList) {
            if (!piece.CompareTag("Piece")) {
                continue;
            }
            GameObject generatePiece = Instantiate(piece.gameObject, new Vector2(piece.transform.position.x + 15, piece.transform.position.y -1), piece.transform.rotation, playerPieceParent);

            playerBattlePieceManager.Add(generatePiece.transform.GetChild(0).GetComponent<BattlePieceManager>());
        }

        // サーバーから受け取ったプレイヤーデータを敵に反映
        EnemyDataResponse enemyDataResponse = JsonConvert.DeserializeObject<EnemyDataResponse>(json);
        // 敵のピースを生成
        foreach (Transform child in enemyPieceParent) {
            Destroy(child.gameObject);
        }

        enemyBattlePieceManager = new List<BattlePieceManager>();

        enemyManager.name = enemyDataResponse.Name;
        enemyManager.characterType = enemyDataResponse.CharacterType;

        SetBattleEnemyCharacter();

        for (int i = 0; i < 49; i++) {
            if (enemyDataResponse.IndexNum[i] == -1) {
                continue;
            }

            Vector2 generatePos = new Vector3(SetupManager.CanPutPiecePosList[i].x + 15, SetupManager.CanPutPiecePosList[i].y + 4.4f);

            GameObject generatePiece = shopManager.CreatePiece(enemyPieceParent, generatePos, enemyDataResponse.PieceAngle[i], false, enemyDataResponse.WeaponAndItemNum[i], enemyDataResponse.PieceFormId[i]);

            generatePiece.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            generatePiece.GetComponent<PieceManager>().enabled = false;

            enemyBattlePieceManager.Add(generatePiece.transform.GetChild(0).GetComponent<BattlePieceManager>());
        }

        // VSPanelの設定
        vsPanel.SetActive(true);

        playerVSNameText.text = playerManager.name;
        switch (playerManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                playerVSCharacters[0].SetActive(true);
                playerVSCharacters[1].SetActive(false);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                playerVSCharacters[0].SetActive(false);
                playerVSCharacters[1].SetActive(true);
                break;
        }

        enemyVSNameText.text = enemyManager.name;
        switch (enemyManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                enemyVSCharacters[0].SetActive(true);
                enemyVSCharacters[1].SetActive(false);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                enemyVSCharacters[0].SetActive(false);
                enemyVSCharacters[1].SetActive(true);
                break;
        }

        Invoke(nameof(StartBattle), 3.0f);
    }

    private void StartBattle() {
        vsPanel.SetActive(false);
        isBattleStart = true;

        // プレイヤーの攻撃開始
        foreach (BattlePieceManager battlePieceManager in this.playerBattlePieceManager) {
            StartCoroutine(battlePieceManager.Action(playerManager, enemyManager));
        }

        // 敵の攻撃開始
        foreach (BattlePieceManager battlePieceManager in this.enemyBattlePieceManager) {
            StartCoroutine(battlePieceManager.Action(enemyManager, playerManager));
        }
    }

    private void GameSet() {
        if (isGameClear) {
            gameSetPanel.SetActive(true);
            gameClearObject.SetActive(true);
        }
        else if (isGameOver) {
            gameSetPanel.SetActive(true);
            gameOverObject.SetActive(true);
        }

        Invoke(nameof(ReturnTitle), 3.0f);
    }

    private void ReturnTitle() {
        SceneManager.LoadScene("GameScene");
    }

    private void Update() {
        if (isGameClear || isGameOver) {
            Invoke(nameof(GameSet), 3.0f);
        }

        if (!isBattleStart) {
            return;
        }

        time += Time.deltaTime;


        // Energy自然回復
        if ((time % 5 >= 0) && (time % 5 <= 1 / (float)Application.targetFrameRate)) {
            EnergyUp();
        }

        // UI更新
        UIUpdate();

        // Hpが0になったら
        if (playerManager.hp <= 0) {
            EndBattle(false);
        }
        else if (enemyManager.hp <= 0) {
            EndBattle(true);
        }
    }

    // バトルが終わったら
    private void EndBattle(bool playerWin) {
        StopAllCoroutines();

        // リザルト表示
        ShowResult(playerWin);

        if(isGameClear || isGameOver) {
            return ;
        }

        isBattleStart = false;
        enabled = false;

        Invoke(nameof(ReturnSetup), 3.0f);
    }

    private void ReturnSetup() {
        battleUICanvas.SetActive(false);
        Camera.main.transform.position = new Vector3(0, 0, -10);
        // セットアップに戻る
        setupManager.Init();
    }

    // リザルト表示
    private void ShowResult(bool playerWin) {
        battleResultPanel.SetActive(true);

        if(playerWin) {
            victoryObject.SetActive(true);
            defeatObject.SetActive(false);

            playerManager.winCount++;

            float addMoney = 12 * (((float)playerManager.currentRound / 10) + 1.0f);

            playerManager.money += Mathf.RoundToInt(addMoney);

            // トロフィーを獲得するアニメーション
            StartCoroutine(GetTrophyAnimation());

            if(playerManager.winCount >= 7) {
                isBattleStart = false;
                isGameClear = true;
            }
        }
        else if(!playerWin) {
            victoryObject.SetActive(false);
            defeatObject.SetActive(true);

            playerManager.battleLife--;

            float addMoney = 10 * (((float)playerManager.currentRound / 10) + 1.0f);

            playerManager.money += Mathf.RoundToInt(addMoney);

            // ライフを失うアニメーション
            StartCoroutine(LostLifeAnimation());

            if(playerManager.battleLife <= 0) {
                isBattleStart = false;
                isGameOver = true;
            }
        }
    }

    // トロフィーを獲得するアニメーション
    private IEnumerator GetTrophyAnimation() {
        winTrophys[playerManager.winCount - 1].SetActive(true);
        winTrophys[playerManager.winCount - 1].transform.localScale = new Vector3(10, 10, 10);

        Vector3 initialScale = winTrophys[playerManager.winCount - 1].transform.localScale;
        float elapsed = 0f;

        while (elapsed < 1.5f) {
            winTrophys[playerManager.winCount - 1].transform.localScale = Vector3.Lerp(initialScale, Vector3.one, elapsed / 1.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        winTrophys[playerManager.winCount - 1].transform.localScale = Vector3.one;
    }

    // ライフを失うアニメーション
    private IEnumerator LostLifeAnimation() {
        playerLifes[playerManager.battleLife].transform.localScale = new Vector3(2, 2, 2);
        Vector3 initialScale = playerLifes[playerManager.battleLife].transform.localScale;
        float elapsed = 0f;

        while (elapsed < 1.0f) {
            playerLifes[playerManager.battleLife].transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / 1.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerLifes[playerManager.battleLife].transform.localScale = Vector3.zero;
        
        playerLifes[playerManager.battleLife].SetActive(false);
    }


    // UI更新
    private void UIUpdate() {

        // Time
        timeText.text = time.ToString("f1") + "s";

        // HP, Energyスライダー
        playerHpSlider.value = playerManager.hp / playerManager.maxHp;
        playerEnergySlider.value = playerManager.energy / playerManager.maxEnergy;

        enemyHpSlider.value = enemyManager.hp / enemyManager.maxHp;
        enemyEnergySlider.value = enemyManager.energy / enemyManager.maxEnergy;

        // HP, Energyテキスト
        playerHpText.text = playerManager.hp.ToString("f1") + " / " + playerManager.maxHp.ToString("f1");
        playerEnergyText.text = playerManager.energy.ToString("f1") + " / " + playerManager.maxEnergy.ToString("f1");

        enemyHpText.text = enemyManager.hp.ToString("f1") + " / " + enemyManager.maxHp.ToString("f1");
        enemyEnergyText.text = enemyManager.energy.ToString("f1") + " / " + enemyManager.maxEnergy.ToString("f1");
    }

    // Energy自然回復
    private void EnergyUp() {
        playerManager.energy += 1.0f;
        enemyManager.energy += 1.0f;
    }

    // バトルログ表示, 非表示トグル
    public void ToggleOnValueChanged() {
        if(battleLogToggle.isOn) {
            battleLog.SetActive(true);
        }
        else if(!battleLogToggle.isOn) {
            battleLog.SetActive(false);
        }
    }

    // バトルログに追加
    public void AddBattleLog(string text, bool isPlayer) {

        if (isPlayer) {
            battleLogText.text += "<align=\"left\">" + time.ToString("f1") + "s:  " + text + "</align>\n";
        }
        else if (!isPlayer) {
            battleLogText.text += "<align=\"right\">" + text + "  :" + time.ToString("f1") + "s</align>\n";
        }
        scrollRect.verticalNormalizedPosition = 0;
        battleLogText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }

    // バトルシーンにプレイヤーのキャラクターを設定
    public void SetBattlePlayerCharacter() {
        switch (playerManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                playerCharacters[0].SetActive(true);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                playerCharacters[1].SetActive(true);
                break;
        }
    }

    // バトルシーンに敵のキャラクターを設定
    public void SetBattleEnemyCharacter() {
        switch (enemyManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                enemyCharacters[0].SetActive(true);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                enemyCharacters[1].SetActive(true);
                break;
        }
    }
}
