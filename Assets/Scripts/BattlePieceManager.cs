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
                battleManager.AddBattleLog("<color=red>エネルギーが足りない...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= energyCost;

            bool isHit = IsAttackHit(target.spd);

            // 行動
            switch (pieceInfo.id) {
                // 鉄剣
                case 1:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>攻撃が外れた</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    float attack = amount * attacker.atk;

                    if(attack < 0) {
                        attack = 0;
                    }

                    target.hp -= attack;

                    battleManager.AddBattleLog(attack + "ダメージを与えた ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 斧
                case 2:

                    break;

                // ハンマー
                case 3:

                    break;

                // ピストル
                case 4:

                    break;


                // 短剣
                case 5:

                    break;

                // 小斧
                case 6:

                    break;

                // リボルバー
                case 7:

                    break;

                // こん棒
                case 8:

                    break;

                // 錆びれた剣
                case 9:

                    break;

                // 木盾
                case 10:

                    break;

                // 銅盾
                case 11:

                    break;

                // 鉄盾
                case 12:

                    break;

                // りんご
                case 13:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("体力を" + amount + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("エネルギーを" + energyUp + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 梨
                case 14:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("体力を" + amount + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("エネルギーを" + energyUp + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);
                    break;

                // 力のポーション
                case 15:

                    break;

                // 素早さのポーション
                case 16:

                    break;

                // 毒のポーション
                case 17:

                    break;

                // ルビー
                case 18:

                    break;

                // サファイア
                case 19:

                    break;

                // 毒龍
                case 20:

                    break;

                // 睡龍
                case 21:

                    break;

                // 痺龍
                case 22:

                    break;

                // チェスト
                case 23:

                    break;

                //
                case 0:

                    break;
            }

            // ステータス調整
            StatusAdjustment(attacker);
        }
    }

    // ステータス調整
    private void StatusAdjustment(CharacterManager attacker) {
        if (attacker.hp > attacker.maxHp) {
            attacker.hp = attacker.maxHp;
        }

        if (attacker.energy > attacker.maxEnergy) {
            attacker.energy = attacker.maxEnergy;
        }
    }

    // 攻撃が当たるか
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
