using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FakeCoinsCollectController : MonoBehaviour
{
    [SerializeField] private TopPanel topPanel;
    
    [SerializeField] private RectTransform coinsText;
    [SerializeField] private Image coinImagePrefab;
    [SerializeField] private TextMeshProUGUI coinsValuePrefab;

    private List<Image> _imagePool = new List<Image>();
    private List<TextMeshProUGUI> _textPool = new List<TextMeshProUGUI>();

    private Vector3 _defaultTextPosition;
    private Vector3 _defaultImagePosition;

    private void Awake()
    {
        _defaultTextPosition = coinsValuePrefab.transform.localPosition;
        _defaultImagePosition = coinImagePrefab.transform.localPosition;
    }

    [ContextMenu("Show")]
    public void ShowAnimation()
    {
        DOVirtual.DelayedCall(0.1f, () => ShowCollectAnimation(Random.Range(100, 1000)));
    }
    
    public void ShowCollectAnimation(int value)
    {
        var text = GetTextFromPool();

        text.transform.localPosition = _defaultTextPosition;
        text.transform.localScale = Vector3.zero;
        text.SetText($"+{value}");
        text.color = new Color(1, 1, 1, 0);
        text.gameObject.SetActive(true);

        text.transform.DOScale(1f, 0.45f);
        DOVirtual.DelayedCall(0.7f, () =>
        {
            text.transform.DOLocalMoveY(text.transform.localPosition.y + 150f, 0.75f);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                text.DOFade(0, 0.6f).OnComplete(() =>
                {
                    text.gameObject.SetActive(false);
                });
            });
        });
        text.DOFade(1f, 0.45f);
        
        DOVirtual.DelayedCall(0.4f, () =>
        {
            var randomCount = Random.Range(6, 10);
            var sum = 0;
            for (int i = 1; i <= randomCount; i++)
            {
                var index = i;
                DOVirtual.DelayedCall(index * 0.1f, () => {
                    var tempValue = value / randomCount;
                
                    if (index == randomCount)
                        tempValue = value - sum;
                    else
                        sum += tempValue;
                
                
                    var coin = GetImageFromPool();

                    coin.transform.localScale = Vector3.zero;
                    coin.transform.localPosition = _defaultImagePosition;
                    coin.transform.localPosition += new Vector3(Random.value * (Random.value > 0.5f ? 1 : -1), Random.value * (Random.value > 0.5f ? 1 : -1)) * Random.Range(50, 100);
                    coin.gameObject.SetActive(true);
                    coin.transform.DOScale(1.2f, 0.4f).OnComplete(() =>
                    {
                        coin.transform.DOScale(1f, 0.1f).OnComplete(() =>
                        {
                            coin.transform.DOLocalMoveY(coin.transform.localPosition.y - 30f, 0.15f).OnComplete(() =>
                            {
                                coin.transform.DOMove(coinsText.position, 0.5f).OnComplete(
                                    () =>
                                    {
                                        coin.gameObject.SetActive(false);
                                        topPanel.UpdateCoinsValueWithParticles(topPanel.CurrentCoinUIValue + tempValue);
                                    });
                            });
                        });
                    });
                });
            }
        });
    }

    private TextMeshProUGUI GetTextFromPool()
    {
        var text = _textPool.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (text == null)
        {
            text = Instantiate(coinsValuePrefab, transform);
            _textPool.Add(text);
        }

        return text;
    }

    private Image GetImageFromPool()
    {
        var image = _imagePool.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (image == null)
        {
            image = Instantiate(coinImagePrefab, transform);
            _imagePool.Add(image);
        }

        return image;
    }
}