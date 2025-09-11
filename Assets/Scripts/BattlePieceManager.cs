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
                battleManager.AddBattleLog("<color=red>エネルギーが足りない...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= pieceInfo.energyCost;

            bool isHit = IsAttackHit(target.spd);

            // 行動
            switch (pieceInfo.items) {
                case WEAPONANDITEMS.Iron_Sword:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>攻撃が外れた</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    float attack = pieceInfo.amount * attacker.atk;

                    if(attack < 0) {
                        attack = 0;
                    }

                    target.hp -= attack;

                    battleManager.AddBattleLog(attack + "ダメージを与えた ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                case (WEAPONANDITEMS.Strawberry):
                    attacker.hp += pieceInfo.amount;
                    attacker.energy += pieceInfo.energyUp;

                    battleManager.AddBattleLog("体力を" + pieceInfo.amount + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("エネルギーを" + pieceInfo.energyUp + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;
            }

            // エネルギー調整
            EnergyAdjustment(attacker);
        }
    }

    // エネルギー調整
    private void EnergyAdjustment(CharacterManager attacker) {
        if(attacker.energy > attacker.maxEnergy) {
            attacker.energy = attacker.maxEnergy;
        }
    }

    // 攻撃が当たるか
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
