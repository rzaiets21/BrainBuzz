using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Image targetGraphic;
    
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;
    
    [SerializeField] private Toggle toggle;

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(ChangeSprite);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(ChangeSprite);
    }

    private void ChangeSprite(bool state)
    {
        targetGraphic.sprite = state ? enabledSprite : disabledSprite;
    }
}
