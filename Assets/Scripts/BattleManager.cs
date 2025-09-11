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
    // �Z�b�g�A�b�v�}�l�[�W���[
    [SerializeField] private SetupManager setupManager;

    // �v���C���[�}�l�[�W���[
    [SerializeField] private PlayerManager playerManager;

    // �G�l�~�[�}�l�[�W���[
    [SerializeField] private EnemyManager enemyManager;

    // �V���b�v�}�l�[�W���[
    [SerializeField] private ShopManager shopManager;

    // �o�g����UI�L�����o�X
    [SerializeField] private GameObject battleUICanvas;

    // �v���C���[�̃s�[�X�̐e�I�u�W�F�N�g
    [SerializeField] private Transform playerPieceParent;

    // �G�̃s�[�X�̐e�I�u�W�F�N�g
    [SerializeField] private Transform enemyPieceParent;

    // ���������v���C���[�̃s�[�X���X�g
    private List<BattlePieceManager> playerBattlePieceManager;

    // ���������G�̃s�[�X���X�g
    private List<BattlePieceManager> enemyBattlePieceManager;

    // �v���C���[��Hp�X���C�_�[
    [SerializeField] private Slider playerHpSlider;

    // �v���C���[��Energy�X���C�_�[
    [SerializeField] private Slider playerEnergySlider;

    // �v���C���[��Hp�e�L�X�g
    [SerializeField] private Text playerHpText;

    // �v���C���[��Energy�e�L�X�g
    [SerializeField] private Text playerEnergyText;

    // �G��Hp�e�L�X�g
    [SerializeField] private Text enemyHpText;

    // �G��Energy�e�L�X�g
    [SerializeField] private Text enemyEnergyText;

    // �G��Hp�X���C�_�[
    [SerializeField] private Slider enemyHpSlider;

    // �G��Energy�X���C�_�[
    [SerializeField] private Slider enemyEnergySlider;

    // �퓬���O�\���g�O��
    [SerializeField] private Toggle battleLogToggle;

    // �퓬���O�X�N���[���r���[
    [SerializeField] private GameObject battleLog;

    // �퓬���O�e�L�X�g
    [SerializeField] private TextMeshProUGUI battleLogText;

    // �퓬���OScrollRect
    [SerializeField] private ScrollRect scrollRect;

    // Time�e�L�X�g
    [SerializeField] private Text timeText;

    // �v���C���[�̃L�����I�u�W�F�N�g
    [SerializeField] private GameObject[] playerCharacters;

    // �G�̃L�����I�u�W�F�N�g
    [SerializeField] private GameObject[] enemyCharacters;

    // VS�p�l���I�u�W�F�N�g
    [SerializeField] private GameObject vsPanel;

    // VS�p�l���p�̃v���C���[���e�L�X�g
    [SerializeField] private Text playerVSNameText;

    // VS�p�l���p�̃G�l�~�[���e�L�X�g
    [SerializeField] private Text enemyVSNameText;

    // �v���C���[��VS�p�l���L�����I�u�W�F�N�g
    [SerializeField] private GameObject[] playerVSCharacters;

    // �G��VS�p�l���L�����I�u�W�F�N�g
    [SerializeField] private GameObject[] enemyVSCharacters;

    // ���U���g�p�p�l��
    [SerializeField] private GameObject battleResultPanel;

    // �����p�I�u�W�F�N�g
    [SerializeField] private GameObject victoryObject;

    // �s�k�p�I�u�W�F�N�g
    [SerializeField] private GameObject defeatObject;

    // �����񐔕\���g���t�B�[
    [SerializeField] private GameObject[] winTrophys;

    // ���C�t�\��
    [SerializeField] private GameObject[] playerLifes;

    // �Q�[���Z�b�g�p�l��
    [SerializeField] private GameObject gameSetPanel;

    // �Q�[���N���A�I�u�W�F�N�g
    [SerializeField] private GameObject gameClearObject;

    // �Q�[���I�[�o�[�I�u�W�F�N�g
    [SerializeField] private GameObject gameOverObject;

    // �o�g�����n�܂��Ă��邩
    private bool isBattleStart;

    // �o�g�����n�܂��Ă���̕b��
    private float time;

    // �Q�[���N���A
    private bool isGameClear;

    // �Q�[���I�[�o�[
    private bool isGameOver;

    private void Start() {
        battleUICanvas.SetActive(false);
        battleResultPanel.SetActive(false);
        enabled = false;

        isBattleStart = false;

        isGameClear = false;
        isGameOver = false;
    }

    // �o�g��������
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

        // �v���C���[�̃f�[�^���T�[�o�[�ɑ��M
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

        // �v���C���[�̃s�[�X�𐶐�
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

        // �T�[�o�[����󂯎�����v���C���[�f�[�^��G�ɔ��f
        EnemyDataResponse enemyDataResponse = JsonConvert.DeserializeObject<EnemyDataResponse>(json);
        // �G�̃s�[�X�𐶐�
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

        // VSPanel�̐ݒ�
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

        // �v���C���[�̍U���J�n
        foreach (BattlePieceManager battlePieceManager in this.playerBattlePieceManager) {
            StartCoroutine(battlePieceManager.Action(playerManager, enemyManager));
        }

        // �G�̍U���J�n
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


        // Energy���R��
        if ((time % 5 >= 0) && (time % 5 <= 1 / (float)Application.targetFrameRate)) {
            EnergyUp();
        }

        // UI�X�V
        UIUpdate();

        // Hp��0�ɂȂ�����
        if (playerManager.hp <= 0) {
            EndBattle(false);
        }
        else if (enemyManager.hp <= 0) {
            EndBattle(true);
        }
    }

    // �o�g�����I�������
    private void EndBattle(bool playerWin) {
        StopAllCoroutines();

        // ���U���g�\��
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
        // �Z�b�g�A�b�v�ɖ߂�
        setupManager.Init();
    }

    // ���U���g�\��
    private void ShowResult(bool playerWin) {
        battleResultPanel.SetActive(true);

        if(playerWin) {
            victoryObject.SetActive(true);
            defeatObject.SetActive(false);

            playerManager.winCount++;

            float addMoney = 12 * (((float)playerManager.currentRound / 10) + 1.0f);

            playerManager.money += Mathf.RoundToInt(addMoney);

            // �g���t�B�[���l������A�j���[�V����
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

            // ���C�t�������A�j���[�V����
            StartCoroutine(LostLifeAnimation());

            if(playerManager.battleLife <= 0) {
                isBattleStart = false;
                isGameOver = true;
            }
        }
    }

    // �g���t�B�[���l������A�j���[�V����
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

    // ���C�t�������A�j���[�V����
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


    // UI�X�V
    private void UIUpdate() {

        // Time
        timeText.text = time.ToString("f1") + "s";

        // HP, Energy�X���C�_�[
        playerHpSlider.value = playerManager.hp / playerManager.maxHp;
        playerEnergySlider.value = playerManager.energy / playerManager.maxEnergy;

        enemyHpSlider.value = enemyManager.hp / enemyManager.maxHp;
        enemyEnergySlider.value = enemyManager.energy / enemyManager.maxEnergy;

        // HP, Energy�e�L�X�g
        playerHpText.text = playerManager.hp.ToString("f1") + " / " + playerManager.maxHp.ToString("f1");
        playerEnergyText.text = playerManager.energy.ToString("f1") + " / " + playerManager.maxEnergy.ToString("f1");

        enemyHpText.text = enemyManager.hp.ToString("f1") + " / " + enemyManager.maxHp.ToString("f1");
        enemyEnergyText.text = enemyManager.energy.ToString("f1") + " / " + enemyManager.maxEnergy.ToString("f1");
    }

    // Energy���R��
    private void EnergyUp() {
        playerManager.energy += 1.0f;
        enemyManager.energy += 1.0f;
    }

    // �o�g�����O�\��, ��\���g�O��
    public void ToggleOnValueChanged() {
        if(battleLogToggle.isOn) {
            battleLog.SetActive(true);
        }
        else if(!battleLogToggle.isOn) {
            battleLog.SetActive(false);
        }
    }

    // �o�g�����O�ɒǉ�
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

    // �o�g���V�[���Ƀv���C���[�̃L�����N�^�[��ݒ�
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

    // �o�g���V�[���ɓG�̃L�����N�^�[��ݒ�
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
