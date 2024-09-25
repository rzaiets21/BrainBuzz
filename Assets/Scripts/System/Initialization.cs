using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainBuzz
{
    public class Initialization : MonoBehaviour
    {
        private void Awake()
        {
            IAPManager.Instance.InitializeIAPManager(InitializeResultCallback);
            DontDestroyOnLoad(this);
        }

        private void InitializeResultCallback(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
        {
            if (status == IAPOperationStatus.Success)
            {
                Debug.Log("InitializeIAPManager");
                foreach (var product in shopProducts)
                {
                    Debug.Log(product.productName + ": " + product.localizedPriceString + ":" + product.price) ;
                }
            }
            else
            {
                Debug.LogError("InitializeIAPManager filed");
            }
        }
    }
}