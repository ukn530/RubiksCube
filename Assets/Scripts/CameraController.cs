using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 _initialPositionPC;
    Vector3 _initialPositionSP;
    Vector3 _initialPosition;
    [SerializeField] GameObject _target;
    [SerializeField] GameObject _canvasSP;
    [SerializeField] GameObject _canvasPC;
    bool _enableInteraction = true;
    bool _isPC = true;

    void Start()
    {
        _initialPositionPC = transform.position;
        _initialPositionSP = Vector3.back * 10f;
        _initialPosition = _initialPositionPC;
        _canvasPC.SetActive(true);
        _canvasSP.SetActive(false);
        _isPC = true;
        CheckScreenRatio();
        StartAnimation();
        Cursor.visible = !Application.isMobilePlatform;
    }

    void Update()
    {
        CheckScreenRatio();
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

    void CheckScreenRatio()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio < 1 && _isPC)
        {
            _initialPosition = _initialPositionSP;
            _canvasPC.SetActive(false);
            _canvasSP.SetActive(true);
            _isPC = false; // Update the state to indicate that we are now in a mobile view
        }
        else if (ratio > 1 && !_isPC)
        {
            _initialPosition = _initialPositionPC;
            _canvasPC.SetActive(true);
            _canvasSP.SetActive(false);
            _isPC = true; // Update the state to indicate that we are now in a PC view
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
    }

    public void ZoomOut(float duration)
    {
        _enableInteraction = false;
        transform.DOMove(transform.position + Vector3.back * 0.3f, duration).SetEase(Ease.InOutExpo);
    }
}
