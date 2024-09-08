using System;
using System.Collections;
using Unity.Advertisement.IosSupport;
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
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();

                while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                    yield return null;
                }
            }
            
            var loading = SceneManager.LoadSceneAsync("MainMenu", new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
            
            while (!loading.isDone)
            {
                progressBar.SetValue(loading.progress);
                yield return null;
            }
        }
    }
}
