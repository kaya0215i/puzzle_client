using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EnemyDataResponse {
    public string Name {  get; set; }
    public CharacterManager.CHARACTER_TYPE CharacterType { get; set; }


    public int[] IndexNum { get; set; }
    public int[] WeaponAndItemNum { get; set; }
    public int[] PieceFormId { get; set; }
    public Quaternion[] PieceAngle { get; set; }
}
