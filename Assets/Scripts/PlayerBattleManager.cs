using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleManager : CharacterManager {
    public int money;
    private int maxBattleLife = 3;
    public int battleLife;
    public int currentRound;
    public int winCount;
    public int rerollCount;

    private void Start() {
        isPlayer = true;
        currentRound = 1;
        rerollCount = 0;
        battleLife = maxBattleLife;
        money = 10;
    }

    public void SetName(string name) {
        this.name = name;
    }

    public void AllReset() {
        base.StatusReset();
        battleLife = maxBattleLife;
        currentRound = 0;
        rerollCount = 1;
        winCount = 0;
    }
}


