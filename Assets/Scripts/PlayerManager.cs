using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager {
    public int money;
    private int maxBattleLife = 3;
    public int battleLife;
    public int currentRound;
    public int winCount;
    public int rerollCount;

    private void Start() {

        name = "Player";
        isPlayer = true;
        currentRound = 1;
        rerollCount = 0;
        battleLife = maxBattleLife;
        money = 10;
    }

    public void AllReset() {
        base.StatusReset();
        battleLife = maxBattleLife;
        currentRound = 0;
        rerollCount = 1;
        winCount = 0;
    }
}


