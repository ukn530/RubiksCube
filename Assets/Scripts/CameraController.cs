using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.position;
        StartAnimation();
    }

    void StartAnimation()
    {
        transform.position = _initialPosition + Vector3.back * 10f;
        transform.DOMove(_initialPosition, 2f).SetEase(Ease.OutCubic);
    }

    public void ZoomIn(float duration)
    {
        transform.DOMove(_initialPosition, duration).SetEase(Ease.InOutExpo);
    }

    public void ZoomOut(float duration)
    {
        transform.DOMove(_initialPosition + Vector3.back * 0.3f, duration).SetEase(Ease.InOutExpo);
    }
}
