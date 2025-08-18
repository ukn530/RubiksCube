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

    bool _isRatio1_2 = false;



    void Update()
    {
        CheckScreenRatio();
    }

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

    void CheckScreenRatio()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio < 1.8f && _isRatio1_2)
        {
            _isRatio1_2 = false;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 256f);
        }
        else if (ratio > 1.8f && !_isRatio1_2)
        {
            _isRatio1_2 = true;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.anchoredPosition = new Vector2(-128f, 80f);
        }
    }
}
