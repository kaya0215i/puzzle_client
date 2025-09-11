using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PieceInfo;

public class BattlePieceManager : MonoBehaviour {
    private PieceInfo pieceInfo;
    private BattleManager battleManager;

    public float amount;
    public float energyUp;
    public float energyCost;
    public float cooltime;

    public enum IsWhoPiece {
        None,
        Player,
        Enemy,
    }

    public IsWhoPiece isWhoPiece = IsWhoPiece.None;

    private void Awake() {
        pieceInfo = GetComponent<PieceInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public void SetStatus() {
        this.amount = pieceInfo.amount;
        this.energyUp = pieceInfo.energyUp;
        this.energyCost = pieceInfo.energyCost;
        this.cooltime = pieceInfo.cooltime;
    }

    public IEnumerator Action(CharacterManager attacker, CharacterManager target) {
        while (true) {
            yield return new WaitForSeconds(cooltime);

            if (attacker.energy < energyCost) {
                battleManager.AddBattleLog("<color=red>�G�l���M�[������Ȃ�...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= energyCost;

            bool isHit = IsAttackHit(target.spd);

            // �s��
            switch (pieceInfo.id) {
                // �S��
                case 1:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>�U�����O�ꂽ</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    float attack = amount * attacker.atk;

                    if(attack < 0) {
                        attack = 0;
                    }

                    target.hp -= attack;

                    battleManager.AddBattleLog(attack + "�_���[�W��^���� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ��
                case 2:

                    break;

                // �n���}�[
                case 3:

                    break;

                // �s�X�g��
                case 4:

                    break;


                // �Z��
                case 5:

                    break;

                // ����
                case 6:

                    break;

                // ���{���o�[
                case 7:

                    break;

                // ����_
                case 8:

                    break;

                // �K�тꂽ��
                case 9:

                    break;

                // �؏�
                case 10:

                    break;

                // ����
                case 11:

                    break;

                // �S��
                case 12:

                    break;

                // ���
                case 13:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("�̗͂�" + amount + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("�G�l���M�[��" + energyUp + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ��
                case 14:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("�̗͂�" + amount + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("�G�l���M�[��" + energyUp + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);
                    break;

                // �͂̃|�[�V����
                case 15:

                    break;

                // �f�����̃|�[�V����
                case 16:

                    break;

                // �ł̃|�[�V����
                case 17:

                    break;

                // ���r�[
                case 18:

                    break;

                // �T�t�@�C�A
                case 19:

                    break;

                // �ŗ�
                case 20:

                    break;

                // ����
                case 21:

                    break;

                // თ�
                case 22:

                    break;

                // �`�F�X�g
                case 23:

                    break;

                //
                case 0:

                    break;
            }

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
}
