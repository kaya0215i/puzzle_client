using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour {
    private void Awake() {
        Application.targetFrameRate = 60;

        DateTime dt = DateTime.Now;
        int iDate = int.Parse(dt.ToString("MMddHHmmss"));
        UnityEngine.Random.InitState(iDate);
    }
}
