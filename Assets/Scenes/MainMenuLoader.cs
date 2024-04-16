using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class MainMenuLoader : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;
        
        private void Start()
        {
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            var loading = SceneManager.LoadSceneAsync("MainMenu", new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
            
            while (!loading.isDone)
            {
                progressBar.SetValue(loading.progress);
                yield return null;
            }
        }
    }
}
