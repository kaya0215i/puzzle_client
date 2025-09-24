using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptionData {
    public CharacterManager.CHARACTER_TYPE CharacterType {  get; set; }
    public int Money { get; set; }
    public int BattleLife { get; set; }
    public int CurrentRound { get; set; }
    public int WinCount { get; set; }
    public int RerollCount { get; set; }

    public List<PieceManagerDTO> InventoryPieceList { get; set; }
    public List<PieceManagerDTO> ShopPieceList { get; set; }
}

public class PieceManagerDTO {
    public int pieceFormId;
    public int itemNum;
    public Quaternion pieceAngle;
    public Vector2 piecePosition;

    public bool isChainedFirstPiece;

    public bool isSet;
    public bool firstPiece;
}
