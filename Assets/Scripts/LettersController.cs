using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LettersController : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private TextMeshProUGUI textComponentPrefab;

    private List<TextMeshProUGUI> _list = new List<TextMeshProUGUI>();

    private int _index = 0;
    
    public TextMeshProUGUI GetTextComponent()
    {
        TextMeshProUGUI text = null;
        
        if(_list.Count == _index)
        {
            text = Instantiate(textComponentPrefab, container);
            _list.Add(text);
        }

        text = _list[_index];
        text.gameObject.SetActive(true);

        _index++;
        
        return text;
    }

    public void ResetState()
    {
        _index = 0;
    }

    public void DisableUnused()
    {
        for (int i = _index; i < _list.Count; i++)
        {
            _list[i].gameObject.SetActive(false);
        }
    }
}
