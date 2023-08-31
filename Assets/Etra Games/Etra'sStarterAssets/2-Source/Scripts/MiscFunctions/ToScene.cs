using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Etra.StarterAssets
{
    public class ToScene : MonoBehaviour
    {
        public void GoToScene(int sceneNum)
        {
            SceneManager.LoadScene(sceneNum);
        }

        public void GoToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
