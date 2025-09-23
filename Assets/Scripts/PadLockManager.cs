using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadLockManager : MonoBehaviour {
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject unLockObject;

    public bool IsLock { get; private set; }

    private PieceManager pieceManager;

    private void Start() {
        IsLock = false;
        pieceManager = GetComponentInParent<PieceManager>();
    }

    public void OnChangeLock() {
        if (IsLock) {
            IsLock = false;

            pieceManager.isLock = false;

            lockObject.SetActive(false);
            unLockObject.SetActive(true);
        }
        else {
            IsLock = true;

            pieceManager.isLock = true;

            lockObject.SetActive(true);
            unLockObject.SetActive(false);
        }
    }
}
