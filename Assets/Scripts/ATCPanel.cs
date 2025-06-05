using DG.Tweening;
using UnityEngine;

public class ATCPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] PlayController _playController;
    Vector3 _initialPosition;
    float _duration = 0.5f;
    float _distance = 50f;

    void Start()
    {
        _canvasGroup.gameObject.SetActive(false);
        _canvasGroup.alpha = 0;
        _initialPosition = transform.localPosition;

    }
    public void Show()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, _duration / 2);
        transform.localPosition = _initialPosition - Vector3.up * _distance;
        transform.DOLocalMoveY(_initialPosition.y, _duration).SetEase(Ease.OutCubic);
        _playController.IsDisableInteraction = true;
    }


    public void Hide()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, _duration / 2);
        transform.localPosition = _initialPosition;
        transform.DOLocalMoveY(_initialPosition.y - _distance, _duration / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            _playController.IsDisableInteraction = false;
            _canvasGroup.gameObject.SetActive(false);
        }); ;

    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
