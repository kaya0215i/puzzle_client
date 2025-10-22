using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class DataManager : MonoBehaviour {
    [SerializeField] private SetupManager setupManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private PlayerBattleManager playerManager;

    //中断データをセーブする
    public void SaveInterruptionData() {
        InterruptionData data = new InterruptionData();
        data.CharacterType = playerManager.characterType;
        data.Money = playerManager.money;
        data.BattleLife = playerManager.battleLife;
        data.CurrentRound = playerManager.currentRound;
        data.WinCount = playerManager.winCount;
        data.RerollCount = playerManager.rerollCount;

        data.InventoryPieceList = new List<PieceManagerDTO>();
        foreach (PieceManager pieceManager in SetupManager.InventoryPieceList) {
            PieceManagerDTO piece = new PieceManagerDTO();
            piece.pieceFormId = pieceManager.pieceFormId;
            piece.itemNum = pieceManager.itemNum;
            piece.pieceAngle = pieceManager.pieceAngle;
            piece.piecePosition = pieceManager.piecePosition;
            piece.isChainedFirstPiece = pieceManager.isChainedFirstPiece;
            piece.isSet = pieceManager.isSet;
            piece.firstPiece = pieceManager.firstPiece;

            data.InventoryPieceList.Add(piece);
        }

        data.ShopPieceList = new List<PieceManagerDTO>();
        foreach (PieceManager pieceManager in shopManager.GetShopPieceList()) {
            PieceManagerDTO piece = new PieceManagerDTO();
            piece.pieceFormId = pieceManager.pieceFormId;
            piece.itemNum = pieceManager.itemNum;
            piece.pieceAngle = pieceManager.pieceAngle;
            piece.piecePosition = pieceManager.piecePosition;
            piece.isChainedFirstPiece = pieceManager.isChainedFirstPiece;
            piece.isSet = pieceManager.isSet;
            piece.firstPiece = pieceManager.firstPiece;

            data.ShopPieceList.Add(piece);
        }

        var settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(data, settings);
        var writer = new StreamWriter(Application.persistentDataPath + "/interruptionData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    // 中断データをロードする
    public bool LoadInterruptionData() {
        if (!ExistsInterruptedData()) {
            return false;
        }
        var reader = new StreamReader(Application.persistentDataPath + "/interruptionData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        try {
            InterruptionData data = JsonConvert.DeserializeObject<InterruptionData>(json);
            playerManager.characterType = data.CharacterType;
            playerManager.money = data.Money;
            playerManager.battleLife = data.BattleLife;
            playerManager.currentRound = data.CurrentRound;
            playerManager.winCount = data.WinCount;
            playerManager.rerollCount = data.RerollCount;
            shopManager.CreateInventoryPieceInterruptionData(data.InventoryPieceList);
            shopManager.CreateShopPieceInterruptionData(data.ShopPieceList);


            return true;
        }
        catch(Exception e) {
            Debug.LogException(e);
            return false;
        }
    }

    // 中断データがあるか
    public bool ExistsInterruptedData() {
        if (!File.Exists(Application.persistentDataPath + "/interruptionData.json")) {
            return false;
        }
        return true;
    }

    // 中断データを削除する
    public void DeleteInterruptedData() {
        File.Delete(Application.persistentDataPath + "/interruptionData.json");
    }
}
