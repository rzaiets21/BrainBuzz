using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonee.MoonSDK
{
    public class SDKStarter : MonoBehaviour
    {
        [SerializeField] private GameObject moonSDK;

        private void Start()
        {
            InitializeMoonSDK();
        }

        private void InitializeMoonSDK()
        {
            moonSDK.SetActive(true);
            DontDestroyOnLoad(moonSDK);
        }
    }
}