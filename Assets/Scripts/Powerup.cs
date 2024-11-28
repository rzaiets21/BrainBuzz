using System;
using Advertising;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    [SerializeField] private ScreensController screensController;

    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip deselectSound;
    [SerializeField] private AudioClip usedSound;
    
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject count;
    [SerializeField] private GameObject buy;
    [SerializeField] private Image icon;
    [SerializeField] private Image _coinsIcon;
    [SerializeField] private Image _adsIcon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private PowerupType powerupType;
    [SerializeField] private int price;

    public event Action<Powerup> onSelect; 
    public event Action onDeselect;

    public event Func<bool> IsInteract; 

    public int Price => price;

    public PowerupType PowerupType => powerupType;

    private bool _selected;

    public void UpdateData()
    {
        PlayerInventory.Instance.Update();
    }
    
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnPowerupClick);
        
        priceText.SetText(price.ToString());
    }

    private void OnEnable()
    {
        PlayerInventory.Instance.OnPowerupValueChanged += UpdatePowerupCount;
        PlayerInventory.Instance.Update();
    }

    private void OnDisable()
    {
        PlayerInventory.Instance.OnPowerupValueChanged -= UpdatePowerupCount;
    }

    private void UpdatePowerupCount(PowerupType powerupType, int value)
    {
        if(this.powerupType != powerupType)
            return;
        
        countText.SetText(value.ToString());
        count.gameObject.SetActive(value > 0);

        buy.gameObject.SetActive(value <= 0);

        if (AdsController.Instance.IsRewardedLoaded() && !PlayerInventory.Instance.HasEnoughCoins(price))
        {
            priceText.gameObject.SetActive(false);
            _coinsIcon.gameObject.SetActive(false);
            _adsIcon.gameObject.SetActive(true);
        }
        else
        {
            priceText.gameObject.SetActive(true);
            _coinsIcon.gameObject.SetActive(true);
            _adsIcon.gameObject.SetActive(false);
        }
    }
    
    public void Deselect(bool used)
    {
        if(!_selected)
            return;

        Select(false);
        
        if(!used)
            AudioController.Instance.Play(SoundType.Sounds, deselectSound);
        else
            AudioController.Instance.Play(SoundType.Sounds, usedSound);
    }
    
    private void OnPowerupClick()
    {
        Select();
    }

    private void Select(bool select = true)
    {
        if (select && _selected)
        {
            Deselect(false);
            return;
        }
        if(select)
        {
            if(IsInteract != null && !IsInteract.Invoke())
                return;

#if !UNITY_EDITOR
            if (!PlayerInventory.Instance.HasPowerup(powerupType) && !PlayerInventory.Instance.HasEnoughCoins(price))
            {
                
                if(AdsController.Instance.IsRewardedLoaded()){
                    AdsController.Instance.ShowRewardedAd(()=> PlayerInventory.Instance.Add(powerupType, 1));    
                }
                else
                {
                    screensController.ShowScreen(Screens.Store);
                }

                return;
            }
#endif
        }
        
        _selected = !_selected;
        
        shine.SetActive(_selected);
        particle.SetActive(_selected);
        //icon.enabled = !_selected;
        
        if(_selected)
        {
            onSelect?.Invoke(this);
            AudioController.Instance.Play(SoundType.Sounds, selectSound);
        }
        else
            onDeselect?.Invoke();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        shine.SetActive(true);
        particle.SetActive(true);
        icon.enabled = false;
        onSelect?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        shine.SetActive(false);
        particle.SetActive(false);
        icon.enabled = true;
        onDeselect?.Invoke();
    }
}

public enum PowerupType
{
    None = 0,
    
    Letter = 10,
    Random = 11,
    Keyboard = 12,
    Line = 13
}