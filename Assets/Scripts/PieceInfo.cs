using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo : MonoBehaviour {

    public int id;
    public new string name;
    public bool isAttack;
    public float amount;
    public float energyUp;
    public float energyCost;
    public float cooltime;
    public string descriptionText;
    public int price;

    public enum WEAPONANDITEMS {
        None,
        Iron_Sword,
        Strawberry,
    }

    public WEAPONANDITEMS items = WEAPONANDITEMS.None;
}
