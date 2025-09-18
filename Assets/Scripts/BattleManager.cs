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
    [SerializeField] private PlayerBattleManager playerManager;

    // �G�l�~�[�}�l�[�W���[
    [SerializeField] private EnemyBattleManager enemyManager;

    // �V���b�v�}�l�[�W���[
    [SerializeField] private ShopManager shopManager;

    // �o�g����UI�L�����o�X
    [SerializeField] private GameObject battleUICanvas;

    // �v���C���[�̃s�[�X�̐e�I�u�W�F�N�g
    [SerializeField] private Transform playerPieceParent;

    // �G�̃s�[�X�̐e�I�u�W�F�N�g
    [SerializeField] private Transform enemyPieceParent;

    // ���������v���C���[�̃s�[�X���X�g
    [NonSerialized] public List<BattlePieceManager> playerBattlePieceManager;

    // ���������G�̃s�[�X���X�g
    [NonSerialized]public List<BattlePieceManager> enemyBattlePieceManager;

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

    // �v���C���[�̃V�[���h�I�u�W�F�N�g
    [SerializeField] private GameObject playerShieldObject;

    // �v���C���[�̃V�[���h�e�L�X�g
    [SerializeField] private Text playerShieldText;

    // �G�̃V�[���h�I�u�W�F�N�g
    [SerializeField] private GameObject enemyShieldObject;

    // �G�̃V�[���h�e�L�X�g
    [SerializeField] private Text enemyShieldText;

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

    // �v���C���[�̃X�e�[�^�X�e�L�X�g
    [SerializeField] private TextMeshProUGUI playerStatusText;

    // �G�̃X�e�[�^�X�e�L�X�g
    [SerializeField] private TextMeshProUGUI enemyStatusText;

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

    private float suddenDeathTimer;
    private float energyTimer;
    private float poisonTimer;
    private float sleepTimer;
    private float stunTimer;

    // �Q�[���N���A
    private bool isGameClear;

    // �Q�[���I�[�o�[
    private bool isGameOver;

    // �T�h���f�X�p
    private bool isSuddenDeath;
    private float suddenDeathValue;

    private void Start() {
        battleUICanvas.SetActive(false);
        battleResultPanel.SetActive(false);
        enabled = false;

        isBattleStart = false;

        isGameClear = false;
        isGameOver = false;

        isSuddenDeath = false;
        suddenDeathValue = 1;
    }

    // �o�g��������
    public IEnumerator Init() {
        enabled = true;
        battleUICanvas.SetActive(true);
        battleResultPanel.SetActive(false);
        gameSetPanel.SetActive(false);
        vsPanel.SetActive(true);

        victoryObject.SetActive(false);
        defeatObject.SetActive(false);
        
        gameClearObject.SetActive(false);
        gameOverObject.SetActive(false);

        isSuddenDeath = false;
        suddenDeathValue = 1;

        time = 0;

        suddenDeathTimer = 0;
        energyTimer = 0;
        poisonTimer = 0;
        sleepTimer = 0;
        stunTimer = 0;

        battleLogText.text = "";

        // �T�[�o�[����󂯎��v���C���[�f�[�^
        EnemyFieldDataResponse enemyDataResponse = new EnemyFieldDataResponse();

        Coroutine getEnemyFieldData = null;

        // �v���C���[�̃f�[�^���T�[�o�[�ɑ��M
        Coroutine registPlayerFieldData = StartCoroutine(NetworkManager.Instance.RegistUserFieldData(
            playerManager,
            result => {
                if (result) {
                    // �G�̃f�[�^���擾
                    getEnemyFieldData = StartCoroutine(NetworkManager.Instance.GetEnemyFieldData(
                        playerManager,
                        resultObjects => {
                            // �T�[�o�[����󂯎�����v���C���[�f�[�^��G�ɔ��f
                            enemyDataResponse = resultObjects;
                        }));
                }
                else {
                    Debug.Log("�t�B�[���h�o�^������ɏI�����܂���ł����B");
                }
            }));

        yield return registPlayerFieldData;
        yield return getEnemyFieldData;

        // �v���C���[�̃s�[�X�𐶐�
        foreach (Transform child in playerPieceParent) {
            Destroy(child.gameObject);
        }

        playerBattlePieceManager = new List<BattlePieceManager>();

        foreach (PieceManager piece in SetupManager.FieldPieceList) {
            GameObject generatePiece = Instantiate(piece.gameObject, new Vector2(piece.transform.position.x + 15, piece.transform.position.y -1), piece.transform.rotation, playerPieceParent);

            playerBattlePieceManager.Add(generatePiece.transform.GetChild(0).GetComponent<BattlePieceManager>());
        }

        // �G�̃s�[�X�𐶐�
        foreach (Transform child in enemyPieceParent) {
            Destroy(child.gameObject);
        }

        enemyBattlePieceManager = new List<BattlePieceManager>();

        enemyManager.name = enemyDataResponse.Name;
        enemyManager.characterType = enemyDataResponse.CharacterType;

        SetBattleEnemyCharacter();

        for (int i = 0; i < 49; i++) {
            GameObject generatePiece = null;
            for (int j = 0; j < 49; j++) {
                if (enemyDataResponse.IndexNum.Count > j) {
                    if (enemyDataResponse.IndexNum[j] == i) {
                        Vector2 generatePos = new Vector3(SetupManager.CanPutPiecePosList[i].x + 15, SetupManager.CanPutPiecePosList[i].y + 4.4f);
                        generatePiece = shopManager.CreatePiece(enemyPieceParent, generatePos, enemyDataResponse.PieceAngle[j], false, enemyDataResponse.ItemNum[j] - 1, enemyDataResponse.PieceFormId[j]);
                        break;
                    }
                }
                else if (j == 48) {
                    generatePiece = Instantiate(setupManager.nullPieceObject.gameObject, Vector3.zero, Quaternion.identity, enemyPieceParent);
                    break;
                }
            }
            generatePiece.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            generatePiece.GetComponent<PieceManager>().enabled = false;

            enemyBattlePieceManager.Add(generatePiece.transform.GetChild(0).GetComponent<BattlePieceManager>());
        }

        // VSPanel�̐ݒ�
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

        playerManager.StatusReset();
        enemyManager.StatusReset();

        UIUpdate();

        yield return new WaitForSeconds(3);

        StartBattle();
    }

    private void StartBattle() {
        vsPanel.SetActive(false);
        isBattleStart = true;

        // �v���C���[�̍U���J�n
        int index = 0;

        foreach (BattlePieceManager battlePieceManager in this.playerBattlePieceManager) {
            battlePieceManager.SetStatus();
            if (battlePieceManager.CompareTag("Item") || battlePieceManager.CompareTag("Weapon")) {
                battlePieceManager.index = index;
            }
            index++;
        }
        foreach (BattlePieceManager battlePieceManager in this.playerBattlePieceManager) {
            StartCoroutine(battlePieceManager.Action(playerManager, enemyManager));
        }

        // �G�̍U���J�n
        index = 0;

        foreach (BattlePieceManager battlePieceManager in this.enemyBattlePieceManager) {
            battlePieceManager.SetStatus();
            if (battlePieceManager.CompareTag("Item") || battlePieceManager.CompareTag("Weapon")) {
                battlePieceManager.index = index;
            }
            index++;
        }
        foreach (BattlePieceManager battlePieceManager in this.enemyBattlePieceManager) {
            StartCoroutine(battlePieceManager.Action(enemyManager, playerManager));
        }
    }
    
    // �Q�[�����I�������
    private IEnumerator GameSet() {
        enabled = false;
        yield return new WaitForSeconds(3);

        if (isGameClear) {
            gameSetPanel.SetActive(true);
            gameClearObject.SetActive(true);
        }
        else if (isGameOver) {
            gameSetPanel.SetActive(true);
            gameOverObject.SetActive(true);
        }

        yield return RankAdjustment(isGameClear);

        yield return new WaitForSeconds(3);
        // �Q�[�����ēǂݍ���
        SceneManager.LoadScene("GameScene");
    }

    private Coroutine RankAdjustment(bool isGameClear) {
        int rankId = NetworkManager.Instance.UserRankId;
        int rankPoint = NetworkManager.Instance.UserRankPoint;
        if(isGameClear) {
            rankPoint += 10 * playerManager.battleLife;

            if(rankPoint >= 100 && rankId < 7) {
                rankPoint -= 100;
                rankId++;
            }
        }
        else {
            rankPoint -= 30 / playerManager.battleLife;
            if(rankPoint < 0 && rankId > 1) {
                rankPoint = 100 - rankPoint;
                rankId--;
            }
            else if(rankPoint < 0 && rankId == 1) {
                rankPoint = 0;
            }
        }

        return StartCoroutine(NetworkManager.Instance.UpdateUser(NetworkManager.Instance.UserName,
            rankId,
            rankPoint,
            result => {
                if (!result) {
                    Debug.Log("���[�U�[���X�V������ɏI�����܂���ł����B");
                }
            }));
    }

    private void Update() {
        if (isGameClear || isGameOver) {
            StartCoroutine(GameSet());
        }

        if (!isBattleStart) {
            return;
        }

        time += Time.deltaTime;

        suddenDeathTimer += Time.deltaTime;
        energyTimer += Time.deltaTime;
        poisonTimer += Time.deltaTime;
        sleepTimer += Time.deltaTime;
        stunTimer += Time.deltaTime;


        // �T�h���f�X 30�b����
        if (suddenDeathTimer >= 30f) {
            suddenDeathTimer = 0;

            AddSystemBattleLog("<color=red>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</color>");
            AddSystemBattleLog("<color=red>�T�h���f�X</color>");
            AddSystemBattleLog("<color=red>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</color>");
            isSuddenDeath = true;
            suddenDeathValue += 2;
            playerManager.atk *= suddenDeathValue;
            enemyManager.atk *= suddenDeathValue;
        }

        // Energy���R�� 3�b����
        if (energyTimer >= 3f) {
            energyTimer = 0;
            EnergyUp();
        }

        // �ł̏��� 1�b����
        if (poisonTimer >= 1f) {
            poisonTimer = 0f;
            // �v���C���[
            if(playerManager.poison > 0) {
                playerManager.poison--;
                playerManager.hp -= 0.3f * suddenDeathValue;

                AddBattleLog("<color=#02FF00><sprite name=poison>�̃_���[�W���󂯂�</color>", true);
            }

            // �G
            if (enemyManager.poison > 0) {
                enemyManager.poison--;
                enemyManager.hp -= 0.3f * suddenDeathValue;
                AddBattleLog("<color=#02FF00><sprite name=poison>�̃_���[�W���󂯂�</color>", false);
            }
        }

        // �����̏��� 1�b����
        if (sleepTimer >= 1f) {
            sleepTimer = 0;
            // �v���C���[
            if (playerManager.sleep > 0) {
                playerManager.sleep--;
            }

            // �G
            if (enemyManager.sleep > 0) {
                enemyManager.sleep--;
            }
        }

        // ��Ⴢ̏��� 1�b����
        if (stunTimer >= 1f) {
            stunTimer = 0f;
            // �v���C���[
            if (playerManager.stun > 0) {
                playerManager.stun--;
            }

            // �G
            if (enemyManager.stun > 0) {
                enemyManager.stun--;
            }
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

            float addMoney = 8 * (((float)playerManager.currentRound / 10) + 1.2f);

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

            float addMoney = 6 * (((float)playerManager.currentRound / 10) + 1.2f);

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

        // �V�[���h
        if(playerManager.shield > 0.0f) {
            playerShieldObject.SetActive(true);
            playerShieldText.text = playerManager.shield.ToString("f1");
        }
        else {
            playerShieldObject.SetActive(false);
            playerShieldText.text = "";
        }

        if (enemyManager.shield > 0.0f) {
            enemyShieldObject.SetActive(true);
            enemyShieldText.text = enemyManager.shield.ToString("f1");
        }
        else {
            enemyShieldObject.SetActive(false);
            enemyShieldText.text = "";
        }

        // �X�e�[�^�X�e�L�X�g
        playerStatusText.text = "";
        enemyStatusText.text = "";

        // ��
        if(playerManager.poison > 0) {
            playerStatusText.text = "<sprite name=poison> " + playerManager.poison;
        }

        if(enemyManager.poison > 0) {
            enemyStatusText.text = "<sprite name=poison> " + enemyManager.poison;
        }

        // ���
        if (playerManager.stun > 0) {
            playerStatusText.text = "<sprite name=stun> " + playerManager.stun;
        }

        if (enemyManager.stun > 0) {
            enemyStatusText.text = "<sprite name=stun> " + enemyManager.stun;
        }

        // ����
        if (playerManager.sleep > 0) {
            playerStatusText.text = "<sprite name=sleep> " + playerManager.sleep;
        }

        if (enemyManager.sleep > 0) {
            enemyStatusText.text = "<sprite name=sleep> " + enemyManager.sleep;
        }
    }

    // Energy���R��
    private void EnergyUp() {
        if (!isSuddenDeath) {
            playerManager.energy += 1.0f;
            enemyManager.energy += 1.0f;
        }
        else {
            playerManager.energy += 2.0f;
            enemyManager.energy += 2.0f;
        }
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

    // �V�X�e���o�g�����O
    public void AddSystemBattleLog(string text) {
        battleLogText.text += "<align=\"center\">" + text + "</align>\n";
        scrollRect.verticalNormalizedPosition = 0;
        battleLogText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }

    // �o�g���V�[���Ƀv���C���[�̃L�����N�^�[��ݒ�
    public void SetBattlePlayerCharacter() {
        switch (playerManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                playerCharacters[0].SetActive(true);
                playerCharacters[1].SetActive(false);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                playerCharacters[0].SetActive(false);
                playerCharacters[1].SetActive(true);
                break;
        }
    }

    // �o�g���V�[���ɓG�̃L�����N�^�[��ݒ�
    public void SetBattleEnemyCharacter() {
        switch (enemyManager.characterType) {
            case CharacterManager.CHARACTER_TYPE.Warrior:
                enemyCharacters[0].SetActive(true);
                enemyCharacters[1].SetActive(false);
                break;

            case CharacterManager.CHARACTER_TYPE.Tank:
                enemyCharacters[0].SetActive(false);
                enemyCharacters[1].SetActive(true);
                break;
        }
    }
}
