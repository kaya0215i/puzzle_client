using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    // WebAPI�̐ڑ����ݒ�
#if DEBUG
    const string API_BASE_URL = "";
#else
    const string API_BASE_URL = "";
#endif

    private string apiToken; // ������APIToken
    private string userName; // ���͂����z��̎����̃��[�U�[ID

    // �v���p�e�B
    public string APIToken {
        get { return this.apiToken; }
    }

    public string UserName {
        get { return this.userName; }
    }

    private static NetworkManager instance;
    public static NetworkManager Instance {
        get {
            if (instance == null) {
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    // �ʐM�p�̊֐�
}
