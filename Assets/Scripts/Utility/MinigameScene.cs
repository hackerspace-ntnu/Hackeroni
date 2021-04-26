using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts 
{
    public class MinigameScene : MonoBehaviour
    {
        public AudioSource musicObjectThatPersistsThroughScenes;
        private static Params loadSceneRegister = null;
    
        [NonSerialized]
        private Params sceneParams;

        public static void LoadMinigameScene(string sceneName) 
        {
            MinigameScene.loadSceneRegister = new Params()
            {
                sceneName = sceneName,
            };
            SceneManager.LoadSceneAsync(sceneName);
        }

        public void Awake() 
        {
            if (loadSceneRegister != null)
            {
                sceneParams = loadSceneRegister;
                
                if (musicObjectThatPersistsThroughScenes != null)
                {
                    musicObjectThatPersistsThroughScenes.time = sceneParams.musicTimestamp;
                }
            }
            loadSceneRegister = null; // the register has served its purpose, clear the state
        }

        public void EndScene () 
        {
            if (sceneParams == null)
                return;
            
            Time.timeScale = 1;

            var loadOp = SceneManager.LoadSceneAsync(0);
        }
        
        public void RestartScene()
        {
            if (sceneParams == null)
                return;

            Time.timeScale = 1;
            loadSceneRegister = sceneParams;

            if (musicObjectThatPersistsThroughScenes != null)
            {
                sceneParams.musicTimestamp = musicObjectThatPersistsThroughScenes.time;
            }
            
            var loadOp = SceneManager.LoadSceneAsync(sceneParams.sceneName);
        }

        [System.Serializable]
        private class Params
        {
            public string sceneName;
            public float musicTimestamp = 0;
        }
    }
}
