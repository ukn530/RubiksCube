using DG.Tweening;
using UnityEngine;

public class ATCPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] PlayController _playController;

    void Start()
    {
        _canvasGroup.gameObject.SetActive(false);
        _canvasGroup.alpha = 0;
    }
    public void Show()
    {
        Debug.Log("ATCPanel Show");
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.2f);
        _playController.IsDisableInteraction = true;
    }


    public void Hide()
    {
        Debug.Log("ATCPanel Hide");
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
        {
            _playController.IsDisableInteraction = false;
            _canvasGroup.gameObject.SetActive(false);
        });
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
