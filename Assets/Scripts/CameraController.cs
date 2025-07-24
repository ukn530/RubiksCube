using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 _initialPosition;
    [SerializeField] GameObject _target;
    bool _enableInteraction = true;

    void Start()
    {
        _initialPosition = transform.position;
        StartAnimation();
    }

    void Update()
    {
        if (_enableInteraction)
        {
            Vector3 mousePos = Input.mousePosition;
            float x = (mousePos.x / Screen.width - 0.5f) * 0.2f;
            float y = (mousePos.y / Screen.height - 0.5f) * 0.2f;
            float offsetAmount = 0.3f;
            Vector3 offset = new Vector3(x * offsetAmount, y * offsetAmount, 0f);
            transform.position = Vector3.Lerp(transform.position, _initialPosition + offset, Time.deltaTime * 3f);

            if (_target != null)
            {
                transform.LookAt(_target.transform.position);
            }
        }
    }

    void StartAnimation()
    {
        transform.position = _initialPosition + Vector3.back * 10f;
        transform.DOMove(_initialPosition, 2f).SetEase(Ease.OutCubic);
    }

    public void ZoomIn(float duration)
    {
        _enableInteraction = true;
        transform.DOMove(_initialPosition, duration).SetEase(Ease.InOutExpo);
        Debug.Log("Zooming in");
    }

    public void ZoomOut(float duration)
    {
        _enableInteraction = false;
        Debug.Log("Zooming out: " + duration);
        transform.DOMove(transform.position + Vector3.back * 0.3f, duration).SetEase(Ease.InOutExpo);
    }
}
