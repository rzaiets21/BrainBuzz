using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductHolder : MonoBehaviour
{
    private const string PriceFormat = "<size=43.94>{0}</size> {1}";
    
    [Serializable]
    private class PowerupHolder
    {
        public PowerupType PowerupType;
        public TextMeshProUGUI countText;
    }

    [SerializeField] private RectTransform container;
    
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject bestPriceLabel;
    [SerializeField] private PowerupHolder[] powerupHolders;

    private ShopProductNames _productName;
    private ProductInfo _productInfo;

    public event Action<ShopProductNames> onBuyButtonClick;

    public RectTransform Container => container;

    public ProductInfo ProductInfo => _productInfo;

    private void OnEnable()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void OnDisable()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClick);
    }

    public void Init(ProductInfo productInfo)
    {
        if (productInfo == null)
        {
            Destroy(gameObject);
            return;
        }

        _productInfo = productInfo;
        _productName = productInfo.name;
        
        if (icon != null)
        {
            icon.sprite = productInfo.icon;

            var iconRect = icon.GetComponent<RectTransform>();
            var position = iconRect.localPosition;
            
            icon.SetNativeSize();
            
            var size = iconRect.sizeDelta;
            size /= 3.13f;
            iconRect.sizeDelta = size;
            
            
            iconRect.localPosition = position;

        }
        if (coinsText != null)
            coinsText.text = productInfo.coins.ToString();
        if (priceText != null)
        {
#if UNITY_EDITOR
            priceText.text = string.Format(PriceFormat, "USD", productInfo.price);
#else
            priceText.text = string.Format(PriceFormat, IAPManager.Instance.GetIsoCurrencyCode(productInfo.name), GetNumericPrice(IAPManager.Instance.GetLocalizedPriceString(productInfo.name)));       
#endif
        }

        if (bestPriceLabel != null)
            bestPriceLabel.SetActive(productInfo.bestPrice);
        if (powerupHolders.Length > 0 && productInfo.powerups.Length > 0)
        {
            for (int i = 0; i < productInfo.powerups.Length; i++)
            {
                var powerupInfo = productInfo.powerups[i];
                var powerupHolder = powerupHolders.First(x => x.PowerupType == powerupInfo.PowerupType);
                powerupHolder.countText.text = powerupInfo.count.ToString();
            }
        }
    }
    
    public static string GetNumericPrice(string price)
    {
        if (string.IsNullOrEmpty(price))
        {
            Debug.LogError("Price string is null or empty.");
            return String.Empty;
        }

        string numericPrice = Regex.Replace(price, @"[^\d.,]", "");
        if (string.IsNullOrEmpty(numericPrice))
        {
            Debug.LogError($"Failed to extract numeric value from {price}");
            return price;
        }

        return numericPrice;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnBuyButtonClick()
    {
        onBuyButtonClick?.Invoke(_productName);
    }
}