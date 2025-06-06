using DG.Tweening;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] float _rotationSpeed = 100f;
    [SerializeField] float _inertiaDamping = 10f; // 慣性の減衰速度
    Vector2 _lastRotationInput = Vector2.zero; // 現在の回転速度
    Vector2 _currentRotationVelocity = Vector2.zero; // 現在の回転速度
    private bool _enabled = true;
    public bool Enabled => _enabled;

    [SerializeField] Texture2D _cursorDefaultTexture;
    [SerializeField] Texture2D _cursorGrabberTexture;

    void Start()
    {
#if UNITY_EDITOR
        _rotationSpeed *= 10;
#endif
        Rotate(3);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(_cursorGrabberTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(_cursorDefaultTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            var rotationInput = Input.mousePositionDelta;
            if (rotationInput.sqrMagnitude > 0.01f || _lastRotationInput.sqrMagnitude < 0.01f)
            {
                _currentRotationVelocity = rotationInput * _rotationSpeed;
            }
            _lastRotationInput = rotationInput;
        }
        else
        {
            _currentRotationVelocity = Vector2.Lerp(_currentRotationVelocity, Vector2.zero, _inertiaDamping * Time.deltaTime);
        }

        float rotationX = _currentRotationVelocity.y * Time.deltaTime;
        float rotationY = -_currentRotationVelocity.x * Time.deltaTime;

        transform.Rotate(Vector3.right, rotationX, Space.World);
        transform.Rotate(Vector3.up, rotationY, Space.World);
    }

    public void Rotate(float index)
    {
        transform.DORotate(transform.localRotation.eulerAngles + Vector3.up * 360 * index, 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutExpo);
    }
}