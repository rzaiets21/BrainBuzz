using System;
using System.Collections;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif
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
#if UNITY_IOS && !UNITY_EDITOR
            var version = new Version(Device.systemVersion);
            if(version >= new Version("14.5"))
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
            }
#endif
            
            var loading = SceneManager.LoadSceneAsync("MainMenu", new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
            
            while (!loading.isDone)
            {
                progressBar.SetValue(loading.progress);
                yield return null;
            }
        }
    }
}
