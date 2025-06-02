using DG.Tweening;
using UnityEngine;

public class ButtonSolve : MonoBehaviour
{

    [SerializeField] PlayController _playController;
    [SerializeField] CanvasGroup _canvasGroup;
    public void OnClickSolveButton()
    {
        _playController.OnClickSolveButton();
        FadeOut();
    }

    void FadeOut()
    {
        _canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
        {
            _canvasGroup.gameObject.SetActive(false);
        });
    }

    public void FadeIn()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.2f);
    }
}
