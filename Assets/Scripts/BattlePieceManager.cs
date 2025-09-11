using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PieceInfo;

public class BattlePieceManager : MonoBehaviour {
    private PieceInfo pieceInfo;
    private BattleManager battleManager;

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

    public IEnumerator Action(CharacterManager attacker, CharacterManager target) {
        while (true) {
            yield return new WaitForSeconds(pieceInfo.cooltime);

            if (attacker.energy < pieceInfo.energyCost) {
                battleManager.AddBattleLog("<color=red>�G�l���M�[������Ȃ�...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= pieceInfo.energyCost;

            bool isHit = IsAttackHit(target.spd);

            // �s��
            switch (pieceInfo.items) {
                case WEAPONANDITEMS.Iron_Sword:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>�U�����O�ꂽ</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    float attack = pieceInfo.amount * attacker.atk;

                    if(attack < 0) {
                        attack = 0;
                    }

                    target.hp -= attack;

                    battleManager.AddBattleLog(attack + "�_���[�W��^���� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                case (WEAPONANDITEMS.Strawberry):
                    attacker.hp += pieceInfo.amount;
                    attacker.energy += pieceInfo.energyUp;

                    battleManager.AddBattleLog("�̗͂�" + pieceInfo.amount + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("�G�l���M�[��" + pieceInfo.energyUp + "�񕜂��� ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;
            }

            // �G�l���M�[����
            EnergyAdjustment(attacker);
        }
    }

    // �G�l���M�[����
    private void EnergyAdjustment(CharacterManager attacker) {
        if(attacker.energy > attacker.maxEnergy) {
            attacker.energy = attacker.maxEnergy;
        }
    }

    // �U���������邩
    private bool IsAttackHit(float targetSpeed) {
        bool isHit;
        float percent = MathF.Round(100.0f / targetSpeed, 2);

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
