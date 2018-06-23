using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneManagement {
    public static Scene GetActiveScene() {
        return SceneManager.GetActiveScene();
    }

    public static void LoadScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
