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
        Debug.Log("BGScreen Show");
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.2f);
    }

    public void Hide()
    {
        Debug.Log("BGScreen Hide");
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
        {
            _canvasGroup.gameObject.SetActive(false);
        });
    }
}
