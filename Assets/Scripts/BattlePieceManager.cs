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

        // 盾は最初に一度行動
        switch (pieceInfo.id) {
            // 木盾
            case 10:
            // 銅盾
            case 11:
            // 鉄盾
            case 12:
                attacker.shield += amount;
                battleManager.AddBattleLog("シールドを" + amount + "獲得した (" + pieceInfo.name + " )", attacker.isPlayer);
                ActionAnimation();
                break;
        }

        while (true) {
            yield return new WaitForSeconds(cooltime);

            if (attacker.sleep > 0) {
                battleManager.AddBattleLog("<color=#5A93FF>ぐっすり眠っているzzz</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
            }

            if (attacker.energy < energyCost) {
                battleManager.AddBattleLog("<color=#FF8000>エネルギーが足りない...</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                continue;
            }

            attacker.energy -= energyCost;

            bool isHit = IsAttackHit(target.spd);
            if (attacker.stun > 0) {
                isHit = IsAttackHit(0.0f, 50.0f);
            }

            float attack;
            float armorTemp;

            // 行動
            switch (pieceInfo.id) {
                // 鉄剣
                case 1:
                // 斧
                case 2:
                // ハンマー
                case 3:
                // 短剣
                case 5:
                // 小斧
                case 6:
                // こん棒
                case 8:
                // 錆びれた剣
                case 9:
                    if (!isHit) {
                        battleManager.AddBattleLog("<color=red>攻撃が外れた</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
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

                    battleManager.AddBattleLog(attack + "ダメージを与えた ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ピストル
                case 4:
                    if (!IsAttackHit(0.0f, 80.0f)) {
                        battleManager.AddBattleLog("<color=red>攻撃が外れた</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
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

                    battleManager.AddBattleLog(attack + "ダメージを与えた ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // リボルバー
                case 7:
                    if (!IsAttackHit(0.0f, 50.0f)) {
                        battleManager.AddBattleLog("<color=red>攻撃が外れた</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
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

                    battleManager.AddBattleLog(attack + "ダメージを与えた ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 木盾
                case 10:
                // 銅盾
                case 11:
                // 鉄盾
                case 12:
                    attacker.shield += amount;
                    battleManager.AddBattleLog("シールドを" + amount + "獲得した (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // りんご
                case 13:
                // 梨
                case 14:
                    attacker.hp += amount;
                    attacker.energy += energyUp;

                    battleManager.AddBattleLog("体力を" + amount + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);
                    battleManager.AddBattleLog("エネルギーを" + energyUp + "回復した ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 力のポーション
                case 15:
                    attacker.atk *= amount;
                    battleManager.AddBattleLog("攻撃力が上がった (" + pieceInfo.name + " )", attacker.isPlayer);

                    yield break;

                // 素早さのポーション
                case 16:
                    attacker.spd *= amount;
                    battleManager.AddBattleLog("素早さが上がった (" + pieceInfo.name + " )", attacker.isPlayer);

                    yield break;

                // 毒のポーション
                case 17:
                    target.poison += (int)amount;
                    battleManager.AddBattleLog("<sprite name=poison>を" + amount + "与えた (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // ルビー
                case 18:
                    // 左
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
                    // 右
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
                    // 上
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
                    // 下
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

                // サファイア
                case 19:
                    // 左
                    if (index % 7 != 0) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index - 1].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index - 1].cooltime /= amount;
                        }
                    }
                    // 右
                    if ((index + 1) % 7 != 0) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index + 1].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index + 1].cooltime /= amount;
                        }

                    }
                    // 上
                    if (!((42 <= index) && (index <= 48))) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index + 7].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index + 7].cooltime /= amount;
                        }

                    }
                    // 下
                    if (!((0 <= index) && (index <= 6))) {
                        if (attacker.isPlayer) {
                            battleManager.playerBattlePieceManager[index - 7].cooltime /= amount;
                        }
                        else {
                            battleManager.enemyBattlePieceManager[index - 7].cooltime /= amount;
                        }
                    }

                    yield break;

                // 毒龍
                case 20:
                    target.poison += (int)amount + (attacker.poison * 2);
                    battleManager.AddBattleLog("<sprite name=poison>を" + ((int)amount + (attacker.poison * 2)) + "与えた (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 睡龍
                case 21:
                    target.sleep += (int)amount;
                    battleManager.AddBattleLog("<sprite name=sleep>を" + amount + "与えた (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // 痺龍
                case 22:
                    target.stun += (int)amount;
                    battleManager.AddBattleLog("<sprite name=stun>を" + amount + "与えた (" + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                // チェスト
                case 23:
                    if (!IsAttackHit(0.0f, 5.0f)) {
                        battleManager.AddBattleLog("<color=red>中身は空っぽだった..</color> ( " + pieceInfo.name + " )", attacker.isPlayer);
                        break;
                    }

                    if (attacker.isPlayer) {
                        playerManager.money += (int)amount;
                    }

                    battleManager.AddBattleLog("コインを" + amount + "獲得 ( " + pieceInfo.name + " )", attacker.isPlayer);

                    break;

                //
                case 0:

                    break;
            }

            StartCoroutine(ActionAnimation());

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

    // 行動アニメーション
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
