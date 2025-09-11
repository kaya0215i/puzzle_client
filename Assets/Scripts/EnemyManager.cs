using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager {
    private void Start() {
        characterType = CHARACTER_TYPE.Warrior;

        name = "Enemy";
        isPlayer = false;

        base.SetStatus();
    }
}
