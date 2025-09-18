using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PieceInfo;
using static UnityEngine.GraphicsBuffer;

public class BattlePieceManager : MonoBehaviour {
    private PieceInfo pieceInfo;
    private BattleManager battleManager;
    private PlayerBattleManager playerManager;

    [NonSerialized] public int index = -1;

    [NonSerialized] public float amount;
    [NonSerialized] public float energyUp;
    [NonSerialized] public float energyCost;
    [NonSerialized] public float cooltime;
    [NonSerialized] public bool isWeapon;

    public enum IsWhoPiece {
        None,
        Player,
        Enemy,
    }

    public IsWhoPiece isWhoPiece = IsWhoPiece.None;

    private void Awake() {
        pieceInfo = GetComponent<PieceInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerBattleManager>();
    }

    public void SetStatus() {
        this.amount = pieceInfo.amount;
        this.energyUp = pieceInfo.energyUp;
        this.energyCost = pieceInfo.energyCost;
        this.cooltime = pieceInfo.cooltime;
        this.isWeapon = pieceInfo.isWeapon;
    }

    public IEnumerator Action(CharacterManager attacker, CharacterManager target) {
        if(index == -1) {
            yield break;
        }

        // ���͍ŏ��Ɉ�x�s��
        switch (pieceInfo.id) {
            // �؏�
            case 10:
            // ����
            case 11:
            // �S��
            case 12:
                attacker.shield += amount;
                battleManager.AddBattleLog("�V�[���h��" + amount + "�l������ (" + pieceInfo.name + " )", attacker.isPlayer);
                ActionAnimation();
                break;
        }

        while (true) {
            yield return new WaitForSeconds(cooltime);

            if (attacker.sleep > 0) {
                battleManager.AddBattleLog("<color=#5A93FF>�������薰���Ă���zzz</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
            }

            if (attacker.energy < energyCost) {
                battleManager.AddBattleLog("<color=#FF8000>�G�l���M�[������Ȃ�...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= energyCost;

            bool isHit = IsAttackHit(target.spd);
            if (attacker.stun > 0) {
                isHit = IsAttackHit(0.0f, 50.0f);
            }

            float attack;
            float armorTemp;

            // �s��
            switch (pieceInfo.id) {
                // �S��
                case 1:
                // ��
                case 2:
                // �n���}�[
                case 3:
                // �Z��
                case 5:
                // ����
                case 6:
                // ����_
                case 8:
                // �K�тꂽ��
                case 9:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>�U�����O�ꂽ</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    attack = amount * attacker.atk;

                    if (attack < 0) {
                        attack = 0;
                    }

                    armorTemp = target.shield;

                    target.shield -= attack;

                    if (target.shield < 0) {
                        target.shield = 0;

                        target.hp -= (attack - armorTemp);
                    }

                    battleManager.AddBattleLog(attack + "�_���[�W��^���� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // �s�X�g��
                case 4:
                    if (!IsAttackHit(0.0f, 80.0f)) {
                        battleManager.AddBattleLog("<color=red>�U�����O�ꂽ</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    attack = amount * attacker.atk;

                    if (attack < 0) {
                        attack = 0;
                    }

                    armorTemp = target.shield;

                    target.shield -= attack;

                    if (target.shield < 0) {
                        target.shield = 0;

                        target.hp -= (attack - armorTemp);
                    }

                    battleManager.AddBattleLog(attack + "�_���[�W��^���� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ���{���o�[
                case 7:
                    if (!IsAttackHit(0.0f, 50.0f)) {
                        battleManager.AddBattleLog("<color=red>�U�����O�ꂽ</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    attack = amount * attacker.atk;

                    if (attack < 0) {
                        attack = 0;
                    }

                    armorTemp = target.shield;

                    target.shield -= attack;

                    if (target.shield < 0) {
                        target.shield = 0;

                        target.hp -= (attack - armorTemp);
                    }

                    battleManager.AddBattleLog(attack + "�_���[�W��^���� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // �؏�
                case 10:
                // ����
                case 11:
                // �S��
                case 12:
                    attacker.shield += amount;
                    battleManager.AddBattleLog("�V�[���h��" + amount + "�l������ (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ���
                case 13:
                // ��
                case 14:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("�̗͂�" + amount + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("�G�l���M�[��" + energyUp + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // �͂̃|�[�V����
                case 15:
                    attacker.atk *= amount;
                    battleManager.AddBattleLog("�U���͂��オ���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    yield break;

                // �f�����̃|�[�V����
                case 16:
                    attacker.spd *= amount;
                    battleManager.AddBattleLog("�f�������オ���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    yield break;

                // �ł̃|�[�V����
                case 17:
                    target.poison += (int)amount;
                    battleManager.AddBattleLog("<sprite name=poison>��" + amount + "�^���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ���r�[
                case 18:
                    // ��
                    if (index % 7 != 0) {
                        if (battleManager.playerBattlePieceManager[index - 1].isWeapon) {
                            if (attacker.isPlayer) {
                                battleManager.playerBattlePieceManager[index - 1].amount *= amount;
                            }
                            else {
                                battleManager.enemyBattlePieceManager[index - 1].amount *= amount;
                            }
                        }
                    }
                    // �E
                    if ((index + 1) % 7 != 0) {
                        if (battleManager.playerBattlePieceManager[index - 1].isWeapon) {
                            if (attacker.isPlayer) {
                                battleManager.playerBattlePieceManager[index + 1].amount *= amount;
                            }
                            else {
                                battleManager.enemyBattlePieceManager[index + 1].amount *= amount;
                            }
                        }
                    }
                    // ��
                    if (!((42 <= index) && (index <= 48))) {
                        if (battleManager.playerBattlePieceManager[index - 1].isWeapon) {
                            if (attacker.isPlayer) {
                                battleManager.playerBattlePieceManager[index + 7].amount *= amount;
                            }
                            else {
                                battleManager.enemyBattlePieceManager[index + 7].amount *= amount;
                            }
                        }
                    }
                    // ��
                    if (!((0 <= index) && (index <= 6))) {
                        if (battleManager.playerBattlePieceManager[index - 1].isWeapon) {
                            if (attacker.isPlayer) {
                                battleManager.playerBattlePieceManager[index - 7].amount *= amount;
                            }
                            else {
                                battleManager.enemyBattlePieceManager[index - 7].amount *= amount;
                            }
                        }
                    }

                    yield break;

                // �T�t�@�C�A
                case 19:
                    // ��
                    if (index % 7 != 0) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index - 1].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index - 1].cooltime /= amount;
                        }
                    }
                    // �E
                    if ((index + 1) % 7 != 0) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index + 1].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index + 1].cooltime /= amount;
                        }

                    }
                    // ��
                    if (!((42 <= index) && (index <= 48))) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index + 7].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index + 7].cooltime /= amount;
                        }

                    }
                    // ��
                    if (!((0 <= index) && (index <= 6))) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index - 7].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index - 7].cooltime /= amount;
                        }
                    }

                    yield break;

                // �ŗ�
                case 20:
                    target.poison += (int)amount + (attacker.poison * 2);
                    battleManager.AddBattleLog("<sprite name=poison>��" + ((int)amount + (attacker.poison * 2)) + "�^���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ����
                case 21:
                    target.sleep += (int)amount;
                    battleManager.AddBattleLog("<sprite name=sleep>��" + amount + "�^���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // თ�
                case 22:
                    target.stun += (int)amount;
                    battleManager.AddBattleLog("<sprite name=stun>��" + amount + "�^���� (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // �`�F�X�g
                case 23:
                    if (!IsAttackHit(0.0f, 5.0f)) {
                        battleManager.AddBattleLog("<color=red>���g�͋���ۂ�����..</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    if (attacker.isPlayer) {
                        playerManager.money += (int)amount;
                    }

                    battleManager.AddBattleLog("�R�C����" + amount + "�l�� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                //
                case 0:

                    break;
            }

            StartCoroutine(ActionAnimation());

            // �X�e�[�^�X����
            StatusAdjustment(attacker);
        }
    }

    // �X�e�[�^�X����
    private void StatusAdjustment(CharacterManager attacker) {
        if (attacker.hp > attacker.maxHp) {
            attacker.hp = attacker.maxHp;
        }

        if (attacker.energy > attacker.maxEnergy) {
            attacker.energy = attacker.maxEnergy;
        }
    }

    // �U���������邩
    private bool IsAttackHit(float targetSpeed, float setPercent = -1) {
        bool isHit;
        
        float percent = MathF.Round(100.0f / targetSpeed, 2);

        if(setPercent != -1) {
            percent = setPercent;
        }

        float rnd = UnityEngine.Random.Range(0.0f, 100.0f);

        if(rnd <= percent || percent >= 100.0f) {
            isHit = true;
        }
        else {
            isHit = false;
        }

        return isHit;
    }

    // �s���A�j���[�V����
    private IEnumerator ActionAnimation() {

        Vector3 originalScale = this.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        float time = 0f;
        while (time < 0.1f) {
            this.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / 1.1f);
            time += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = targetScale;

        time = 0f;
        while (time < 0.1f) {
            this.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / 1.1f);
            time += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = originalScale;

    }
}
