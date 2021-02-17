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
                callbackWhenMinigameSceneFinishedUnloading = (outcome) => 
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
                
                var musicObject = GameObject.Find("Music");
                if (musicObject != null)
                {
                    var audioSource = musicObject.GetComponent<AudioSource>();
                    
                    if (audioSource != null)
                    {
                        audioSource.time = sceneParams.musicTimestamp;
                    }
                }
            }
            loadSceneRegister = null; // the register has served its purpose, clear the state
        }

        public void EndScene (Outcome outcome) 
        {
            if (sceneParams == null)
                return;

            var loadOp = SceneManager.UnloadSceneAsync(sceneParams.sceneName);

            loadOp.completed += (o) => {
                if (sceneParams.callbackWhenMinigameSceneFinishedUnloading != null) 
                {
                    sceneParams.callbackWhenMinigameSceneFinishedUnloading(outcome);
                }
                sceneParams.callbackWhenMinigameSceneFinishedUnloading = null; // Protect against double calling;
            };
        }
        
        public void RestartScene()
        {
            if (sceneParams == null)
                return;

            loadSceneRegister = sceneParams;

            var musicObject = GameObject.Find("Music");
            if (musicObject != null)
            {
                var audioSource = musicObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    sceneParams.musicTimestamp = audioSource.time;
                }
            }
            
            var loadOp = SceneManager.UnloadSceneAsync(sceneParams.sceneName);

            loadOp.completed += (o) => {
                SceneManager.LoadSceneAsync(sceneParams.sceneName, LoadSceneMode.Additive);
            };
        }

        [System.Serializable]
        private class Params
        {
            public System.Action<Outcome> callbackWhenMinigameSceneFinishedUnloading;
            public string sceneName;
            public float musicTimestamp = 0;
        }
        [System.Serializable]
        public class Outcome
        {
            public int highscore;
        }
    }
}
