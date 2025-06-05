using DG.Tweening;
using UnityEngine;

public class BGScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup.gameObject.SetActive(false);
        _canvasGroup.alpha = 0;
    }

    public void Show()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.25f);
    }

    public void Hide()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, 0.25f).SetDelay(0.25f).OnComplete(() =>
        {
            _canvasGroup.gameObject.SetActive(false);
        });
    }
}
