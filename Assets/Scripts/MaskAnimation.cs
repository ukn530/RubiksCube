using UnityEngine;
using DG.Tweening;

public class MaskAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _maskRectTransform;
    [SerializeField] private float _delay = 0;
    private float _duration = 1f;


    void Start()
    {
        AnimateMask();
    }

    public void AnimateMask()
    {
        if (_maskRectTransform != null)
        {
            float originalWidth = _maskRectTransform.sizeDelta.x;
            float originalXPosition = _maskRectTransform.anchoredPosition.x;
            _maskRectTransform.sizeDelta = new Vector2(0, _maskRectTransform.sizeDelta.y);
            _maskRectTransform.anchoredPosition = new Vector2(originalXPosition + 50f, _maskRectTransform.anchoredPosition.y);
            _maskRectTransform.DOSizeDelta(new Vector2(originalWidth, _maskRectTransform.sizeDelta.y), _duration)
                .SetEase(Ease.InOutExpo)
                .SetDelay(_delay);
            _maskRectTransform.DOAnchorPosX(originalXPosition, _duration)
                .SetEase(Ease.InOutExpo)
                .SetDelay(_delay);
        }
    }
}
