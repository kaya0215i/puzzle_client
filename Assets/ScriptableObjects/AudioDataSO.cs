using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JigsawBattle/AudioDataSO")]
public class AudioDataSO : ScriptableObject {
    public List<AudioData> audioDataList = new List<AudioData>();
}

[System.Serializable]
public class AudioData {
    public string name;
    public AudioClip clip;
}