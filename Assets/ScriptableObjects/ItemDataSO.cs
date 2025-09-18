using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "JigsawBattle/ItemDataSO")]
public class ItemDataSO : ScriptableObject {
    public List<ItemData> itemDataList = new List<ItemData>();

    //ItemDataSO‚ª•Û‘¶‚µ‚Ä‚ ‚éêŠ‚ÌƒpƒX
    public const string PATH = "ItemDataSO";

    private static ItemDataSO instance;
    public static ItemDataSO Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<ItemDataSO>(PATH);
            }
            return instance;
        }
    }
}

[System.Serializable]
public class ItemData {
    public int id;
    public string name;
    public bool isWeapon;
    public float amount;
    public float energyUp;
    public float energyCost;
    public float cooltime;
    public string descriptionText;
    public int price;
}