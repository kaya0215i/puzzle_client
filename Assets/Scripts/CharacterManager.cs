using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class CharacterManager : MonoBehaviour {
    public new string name;
    public float maxHp;
    public float hp;
    public float atk;
    public float spd;
    public float maxEnergy;
    public float energy;
    public float armor;

    public bool isPlayer;

    public CHARACTER_TYPE characterType = CHARACTER_TYPE.None;

    public enum CHARACTER_TYPE {
        None,
        Warrior,
        Tank,
    }

    public void StatusReset() {
        switch (characterType) {
            case CHARACTER_TYPE.Warrior:
                maxHp = 25.0f;
                hp = maxHp;
                atk = 1.2f;
                spd = 1.1f;
                maxEnergy = 5.0f;
                energy = maxEnergy;
                armor = 0.0f;

                break;

            case CHARACTER_TYPE.Tank:
                maxHp = 32.0f;
                hp = maxHp;
                atk = 0.9f;
                spd = 0.9f;
                maxEnergy = 8.0f;
                energy = maxEnergy;
                armor = 0.0f;

                break;
        }
    }
}
