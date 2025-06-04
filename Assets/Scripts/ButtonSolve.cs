using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSolve : MonoBehaviour
{

    [SerializeField] PlayController _playController;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] GameObject _textImage;
    [SerializeField] GameObject _iconImage;
    [SerializeField] Button _solveButton;

    public void OnClickSolveButton()
    {
        _solveButton.interactable = false;
        _playController.OnClickSolveButton();
        Processing();
    }

    async void Processing()
    {
        _textImage.SetActive(false);
        _iconImage.SetActive(true);
        float angle = 0;
        while (_iconImage.activeInHierarchy)
        {
            _iconImage.transform.localRotation = Quaternion.Euler(0, 0, -angle * 30);
            angle++;
            await Awaitable.WaitForSecondsAsync(0.05f);
        }
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
        {
            _textImage.SetActive(true);
            _iconImage.SetActive(false);
            _canvasGroup.gameObject.SetActive(false);
        });
    }

    public void FadeIn()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.5f).SetDelay(0.5f).OnComplete(() =>
        {
            _textImage.SetActive(true);
            _iconImage.SetActive(false);
            _solveButton.interactable = true;
        });
    }
}
