using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductHolder : MonoBehaviour
{
    private const string PriceFormat = "<size=43.94>USD</size> {0}";
    
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

    public event Action<ShopProductNames> onBuyButtonClick;

    public RectTransform Container => container;

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
            priceText.text = string.Format(PriceFormat, productInfo.price);
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

    private void OnBuyButtonClick()
    {
        onBuyButtonClick?.Invoke(_productName);
    }
}