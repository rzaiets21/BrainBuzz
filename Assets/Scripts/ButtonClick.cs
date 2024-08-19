using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClick : MonoBehaviour
{
    [SerializeField] private AudioClip soundClip;
    private Button _button;

    private Animator _animator;

    private void Awake()
    {
        _button = GetComponent<Button>();
        TryGetComponent(out _animator);
    }

    private void OnEnable()
    {
        if(_animator != null)
            _animator.SetTrigger("Normal");
        
        _button.onClick.AddListener(PlaySound);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PlaySound);
        
        if(_animator != null)
            _animator.SetTrigger("Normal");
    }

    private void PlaySound()
    {
        AudioController.Instance.Play(SoundType.Sounds, soundClip);
    }
}
