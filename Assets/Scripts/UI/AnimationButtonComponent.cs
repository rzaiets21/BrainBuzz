using DG.Tweening;
using UnityEngine;

namespace BrainBuzz
{
    public class AnimationButtonComponent : MonoBehaviour
    {
        [SerializeField] private RectTransform _button;
        private bool _isAnimating = false;

        public void OnClick()
        {
            if (_isAnimating)
            {
                return;
            }

            _isAnimating = true;
            _button.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.15f).OnComplete(() => { _button.DOScale(Vector3.one, 0.15f).OnComplete(() => _isAnimating = false); });
        }
    }
}