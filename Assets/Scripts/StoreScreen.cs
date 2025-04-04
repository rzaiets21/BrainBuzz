using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class StoreScreen : Screen
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject restoreButton;
    
    [SerializeField] private Ease ease;
    [SerializeField] private float showDelay;
    [SerializeField] private float showDuration;
    [SerializeField] private ShopDefinition shopDefinition;

    [SerializeField] private int countToShow;
    [SerializeField] private Button moreButton;
    
    [SerializeField] private RectTransform container;
    [SerializeField] private ProductHolder productHolderPrefab;
    [SerializeField] private ProductHolder removeAdsProductHolder;

    private List<ProductHolder> _productHolders;
    
    private void Awake()
    {
        IAPManager.Instance.InitializeIAPManager((arg0, s, list) => { });
        InitProducts();
    }

    private void OnDestroy()
    {
        if (_productHolders is { Count: > 0 })
        {
            foreach (var productHolder in _productHolders)
            {
                productHolder.onBuyButtonClick -= OnBuyButtonClick;
            }
        }
    }

    public void OnClickMore()
    {
        moreButton.transform.parent.gameObject.SetActive(false);
        foreach (var productHolder in _productHolders)
        {
            productHolder.gameObject.SetActive(true);
        }
    }
    
    private void InitProducts()
    {
        _productHolders = new List<ProductHolder>();
        
        var productList = shopDefinition.products;
        foreach (var productInfo in productList)
        {
            //if (productInfo.adsFree && AdsManager.Instance.IsAdsFree()) // TODO temp disabled
            if (productInfo.adsFree)
            {
                continue;
            }
            
            ProductHolder productHolder = productInfo.adsFree ? removeAdsProductHolder : Instantiate(productHolderPrefab, container);
            productHolder.Init(productInfo);
            productHolder.gameObject.SetActive(true);
            productHolder.onBuyButtonClick += OnBuyButtonClick;
            
            if (_productHolders.Count == countToShow)
            {
                moreButton.transform.parent.SetAsLastSibling();
            }

            _productHolders.Add(productHolder);
        }
        
        removeAdsProductHolder.transform.SetAsLastSibling();
        
#if UNITY_IOS
        restoreButton.SetActive(true);
        restoreButton.transform.SetAsLastSibling();
#endif
    }

    protected override void OnShown()
    {
        SetInteractable(false);
        moreButton.transform.parent.gameObject.SetActive(_productHolders.Count > countToShow);

        var offset = container.rect.width / 2f;
        
        var moreButtonRect = moreButton.GetComponent<RectTransform>();
        moreButtonRect.anchoredPosition = new Vector2(offset, moreButtonRect.anchoredPosition.y);
        var i = 0;
        for (i = 0; i < _productHolders.Count; i++)
        {
            var productHolder = _productHolders[i];
            productHolder.gameObject.SetActive(i < countToShow);
            if (!productHolder.gameObject.activeSelf)
                continue;

            productHolder.Container.anchoredPosition = new Vector2(offset, productHolder.Container.anchoredPosition.y);
            DOVirtual.DelayedCall((i + 1) * showDelay, () =>
            {
                productHolder.Container.DOAnchorPosX(0, showDuration).SetEase(ease);
            });
        }

        if (_productHolders.Count > countToShow)
        {
            DOVirtual.DelayedCall((countToShow + 1) * showDelay, () =>
            {
                moreButtonRect.DOAnchorPosX(0, showDuration).SetEase(ease).OnComplete(() =>
                {
                    SetInteractable(true);
                });
            });
        }
        else
        {
            DOVirtual.DelayedCall((countToShow + 1) * showDelay, () =>
            {
                SetInteractable(true);
            });
        }
    }

    protected override void OnHidden()
    {
        scrollRect.normalizedPosition = Vector2.one;
    }

    public void OnClickRestoreButton()
    {
        Debug.Log("Clicked Restore button");
        IAPManager.Instance.RestorePurchases(OnPurchaseComplete);
    }
    
    private void OnBuyButtonClick(ShopProductNames productName)
    {
        Debug.Log("Buy");
        IAPManager.Instance.BuyProduct(productName, OnPurchaseComplete);
    }

    private void OnPurchaseComplete(IAPOperationStatus status, string arg1, StoreProduct storeProduct)
    {
        if (status == IAPOperationStatus.Fail)
            return;

        var productName= storeProduct.productName;
        var product = shopDefinition.products.First(x => x.name.ToString() == productName);
        ConsumeProduct(product);
    }

    private void ConsumeProduct(ProductInfo productInfo)
    {
        var coinsValue = productInfo.coins;
        PlayerInventory.Instance.Add(coinsValue);

        foreach (var productInfoPowerup in productInfo.powerups)
        {
            PlayerInventory.Instance.Add(productInfoPowerup.PowerupType, productInfoPowerup.count);
        }

        if (productInfo.adsFree)
        {
            PlayerPrefs.SetInt("AdsFree", 1);
            PlayerPrefs.Save();
            //AdsManager.Instance.DestroyBannerView();
            var adsfree = _productHolders.FirstOrDefault(c => c.ProductInfo.adsFree);
            if (adsfree != null)
            {
                adsfree.onBuyButtonClick -= OnBuyButtonClick;
                _productHolders.Remove(adsfree);
                adsfree.Destroy();
            }
        }
    }
}
