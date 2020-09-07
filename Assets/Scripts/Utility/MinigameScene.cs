using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts 
{
    public class MinigameScene : MonoBehaviour
    {
        private static Params loadSceneRegister = null;
    
        [NonSerialized]
        private Params sceneParams;

        public static void LoadMinigameScene(string sceneName, System.Action<Outcome> callbackWhenMinigameSceneExits) 
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            MinigameScene.loadSceneRegister = new Params()
            {
                sceneName = sceneName,
                callbackWhenMinigameSceneExits = (outcome) => 
                    { 
                        foreach(var rootObject in rootObjects)
                        {
                            if (rootObject != null)
                                rootObject.SetActive(true);
                        }
                        callbackWhenMinigameSceneExits(outcome);
                    },
            };
            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            loadOp.completed += (o) => {
                foreach(var rootObject in rootObjects)
                {
                    if (rootObject != null)
                        rootObject.SetActive(false);
                }
            };
        }

        public void Awake() 
        {
            if (loadSceneRegister != null)
            {
                sceneParams = loadSceneRegister;
            }
            loadSceneRegister = null; // the register has served its purpose, clear the state
        }

        public void EndScene (Outcome outcome) 
        {
            if (sceneParams == null)
                return;

            SceneManager.UnloadSceneAsync(sceneParams.sceneName);
            if (sceneParams.callbackWhenMinigameSceneExits != null) 
            {
                sceneParams.callbackWhenMinigameSceneExits(outcome);
            }
            sceneParams.callbackWhenMinigameSceneExits = null; // Protect against double calling;
        }

        [System.Serializable]
        private class Params
        {
            public System.Action<Outcome> callbackWhenMinigameSceneExits;
            public string sceneName;
        }
        [System.Serializable]
        public class Outcome
        {
            public int highscore;
        }
    }
}
