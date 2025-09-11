using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataRequest {
    public string Name { get; set; }
    public CharacterManager.CHARACTER_TYPE CharacterType { get; set; }
    public int currentRound { get; set; }

    public int[] IndexNum { get; set; }
    public int[] WeaponAndItemNum { get; set; }
    public int[] PieceFormId { get; set; }
    public Quaternion[] PieceAngle { get; set; }
}
