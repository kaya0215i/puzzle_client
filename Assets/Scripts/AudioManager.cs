using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    // 0 = BGM, 1 = SE
    private AudioSource BGMSource;
    private AudioSource SESource;
    private AudioDataSO audioDataSO;

    //AudioDataSOが保存してある場所のパス
    public const string PATH = "AudioDataSO";

    private static AudioManager instance;
    public static AudioManager Instance {
        get {
            if (instance == null) {
                GameObject gameObj = new GameObject("AudioManager");
                instance = gameObj.AddComponent<AudioManager>();
                instance.BGMSource = gameObj.AddComponent<AudioSource>();
                instance.SESource = gameObj.AddComponent<AudioSource>();
                instance.audioDataSO = Resources.Load<AudioDataSO>(PATH);
                instance.BGMSource.loop = true;
                instance.BGMSource.volume = 0.5f;
                instance.SESource.volume = 0.5f;
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    // BGMボリューム変更
    public void ChangeBGMValume(float value) {
        BGMSource.volume = value;
    }

    // SEボリューム変更
    public void ChangeSEValume(float value) {
        SESource.volume = value;
    }

    // BGM変更して再生
    public void ChangeBGM(string clipName) {
        AudioClip audioClip = audioDataSO.audioDataList.Find(x => x.name == clipName).clip;

        if (audioClip != null) {
            BGMSource.clip = audioClip;
            BGMSource.Play();
        }
    }

    // BGM停止
    public void StopBGM() {
        BGMSource.Stop();
    }

    // SE再生
    public void PlayOneShot(string clipName) {
        AudioClip audioClip = audioDataSO.audioDataList.Find(x => x.name == clipName).clip;

        if (audioClip != null) {
            SESource.PlayOneShot(audioClip);
        }
    }
}
