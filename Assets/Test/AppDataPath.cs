using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppDataPath : MonoBehaviour {

    private void Start() {
        Debug.Log(string.Format("--- Application.dataPath: {0}---", Application.dataPath));
    }
}
