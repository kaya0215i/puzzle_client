using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShopManager : MonoBehaviour {
    // �V���b�v�̐e�I�u�W�F�N�g
    [SerializeField] private Transform shopParent;

    // �s�[�X�̃I�u�W�F�N�g
    [SerializeField] private GameObject shopPieces;

    // ���큕�A�C�e���v���n�u���X�g
    [SerializeField] private List<GameObject> itemPrefabs;

    // �V���b�v�ɕ��ԃs�[�X�̍��W���X�g
    private List<Vector2> shopPiecePosList = new List<Vector2>() {
        new Vector2(-1.12f, 2.42f),
        new Vector2(1.12f, 2.42f),
        new Vector2(-1.8f, 1.35f),
        new Vector2(0f, 1.5f),
        new Vector2(1.8f, 1.35f)
    };

    // �s�[�X�̌`(�X�v���C�g)���X�g
    [SerializeField] private List<Sprite> pieceFormList;

    // �v���C�X�J�[�h�̃v���n�u
    [SerializeField] private GameObject priceCardPrefab;

    // �v���C�X�J�[�h�̐e�I�u�W�F�N�g
    [SerializeField] private Transform priceCardParend;

    // �v���C���[�}�l�[�W���[
    [SerializeField] private PlayerBattleManager playerManager;

    public void SortShop() {
        // �V���b�v�̐e�I�u�W�F�N�g����ɂ���
        foreach (Transform child in shopParent) {
            Destroy(child.gameObject);
        }

        // �v���C�X�J�[�h�̐e�I�u�W�F�N�g����ɂ���
        foreach (Transform child in priceCardParend) {
            Destroy(child.gameObject);
        }

        // �V���b�v�̐e�I�u�W�F�N�g�ɓ����
        foreach (Vector2 pos in shopPiecePosList) {
            int rnd = UnityEngine.Random.Range(0, 0);
            float rndX = UnityEngine.Random.Range(-0.15f, 0.15f);
            float rndY = UnityEngine.Random.Range(-0.15f, 0.15f);

            Vector2 shopPos = new Vector2(pos.x + rndX, pos.y + rndY);

            // �s�[�X�쐬
            PieceManager createdPiece = CreatePiece(shopParent, shopPos, Quaternion.identity, true).GetComponent<PieceManager>();
            createdPiece.inShop = true;
            createdPiece.shopPos = shopPos;

            // �v���C�X�J�[�h�̍쐬
            Vector2 priceCardPos = new Vector2(createdPiece.shopPos.x, createdPiece.shopPos.y - 0.5f);
            GameObject createdPriceCard = Instantiate(priceCardPrefab, priceCardPos, Quaternion.identity, priceCardParend);

            Text priceText = createdPriceCard.transform.GetChild(0).GetComponent<Text>();
            priceText.text = "" + createdPiece.transform.GetChild(0).GetComponent<PieceInfo>().price;
        }

        // �����[���p�v���C�X�J�[�h�̍쐬
        GameObject createdRerollPriceCard = Instantiate(priceCardPrefab, new Vector2(2.37f, 3.9f), Quaternion.identity, priceCardParend);

        Text rerollPriceText = createdRerollPriceCard.transform.GetChild(0).GetComponent<Text>();
        rerollPriceText.text = "" + ( 1 + (int)(playerManager.rerollCount * 0.1f) + (playerManager.currentRound - 1));
    }

    // �s�[�X�쐬
    public GameObject CreatePiece(Transform parent, Vector3 position, Quaternion pieceAngle, bool isPlayer, int itemNum = -1, int pieceFormNum = -1) {
        int rnd = UnityEngine.Random.Range(0, 0);

        // �s�[�X���쐬
        GameObject pieceObject = Instantiate(shopPieces, position, pieceAngle, parent);

        PieceManager pieceManager = pieceObject.GetComponent<PieceManager>();

        // �A�C�e����ݒ�
        int num = itemNum;

        if (itemNum == -1) {
            num = UnityEngine.Random.Range(0, itemPrefabs.Count);
        }

        pieceManager.itemNum = num + 1;

        // �A�C�e�����쐬
        GameObject pieceItems = Instantiate(itemPrefabs[num], pieceObject.transform.position, pieceObject.transform.rotation, pieceObject.transform);
        // �A�C�e������ݒ�
        PieceInfo pieceInfo = pieceItems.GetComponent<PieceInfo>();
        pieceInfo.id = ItemDataSO.Instance.itemDataList[num].id;
        pieceInfo.name = ItemDataSO.Instance.itemDataList[num].name;
        pieceInfo.isWeapon = ItemDataSO.Instance.itemDataList[num].isWeapon;
        pieceInfo.amount = ItemDataSO.Instance.itemDataList[num].amount;
        pieceInfo.energyUp = ItemDataSO.Instance.itemDataList[num].energyUp;
        pieceInfo.energyCost = ItemDataSO.Instance.itemDataList[num].energyCost;
        pieceInfo.cooltime = ItemDataSO.Instance.itemDataList[num].cooltime;
        pieceInfo.descriptionText = ItemDataSO.Instance.itemDataList[num].descriptionText;
        pieceInfo.price = ItemDataSO.Instance.itemDataList[num].price;

        if (isPlayer) {
            pieceItems.GetComponent<BattlePieceManager>().isWhoPiece = BattlePieceManager.IsWhoPiece.Player;
        }
        else if (!isPlayer) {
            pieceItems.GetComponent<BattlePieceManager>().isWhoPiece = BattlePieceManager.IsWhoPiece.Enemy;
        }
            
        // �s�[�X�̌`��ύX
        num = pieceFormNum;

        if (num == -1) {
            num = UnityEngine.Random.Range(0, pieceFormList.Count);
        }

        pieceManager.GetComponent<SpriteRenderer>().sprite = pieceFormList[num];

        if (num == 0) {
            pieceManager.dentDirections = new bool[4] { false, false, false, false };
            pieceManager.pieceFormId = 0;
        }
        else if (num == 1) {
            pieceManager.dentDirections = new bool[4] { true, false, false, false };
            pieceManager.pieceFormId = 1;
        }
        else if (num == 2) {
            pieceManager.dentDirections = new bool[4] { true, true, false, false };
            pieceManager.pieceFormId = 2;
        }
        else if (num == 3) {
            pieceManager.dentDirections = new bool[4] { false, true, false, true };
            pieceManager.pieceFormId = 3;
        }
        else if (num == 4) {
            pieceManager.dentDirections = new bool[4] { true, true, false, true };
            pieceManager.pieceFormId = 4;
        }
        else if (num == 5) {
            pieceManager.dentDirections = new bool[4] { true, true, true, true };
            pieceManager.pieceFormId = 5;
        }

        return pieceObject;
    }
}
