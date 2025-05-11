using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GrabberController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    List<GameObject> _grabbedObjects = new List<GameObject>();
    [SerializeField] PlayController _playController;
    Vector3 _forward;
    Quaternion _baseRotation;
    // public bool IsRotating => _isRotating;
    bool _isClicking;
    bool _isDragging;

    public enum State
    {
        Base,
        PreRotated,
        Rotating,
    }

    State _state = State.Base;
    public State CurrentState => _state;

    void Start()
    {
        _forward = transform.right;
        _baseRotation = transform.localRotation;
        // SetCellBaseTransform();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // print($"オブジェクト {name} がクリックされたよ！");
        if (!_isDragging)
        {
            _playController.ClickedGrabber(this);
        }
        _isClicking = false;
        _isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // print($"オブジェクト {name} にマウスが乗ったよ！");
        // PreRotateFace();
        _playController.OnGrabber(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // print($"オブジェクト {name} からマウスが離れたよ！");
        // ResetRotation();
        _playController.OffGrabber(this);

    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // print($"オブジェクト {name} にマウスが移動したよ！");
        // PreRotateFace();
        _isDragging = _isClicking;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // print($"オブジェクト {name} がクリックされたよ！");
        _isClicking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // print($"オブジェクト {name} のクリックが離れたよ！");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cell") && !_grabbedObjects.Contains(other.gameObject))
        {
            _grabbedObjects.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cell") && _grabbedObjects.Contains(other.gameObject))
        {
            _grabbedObjects.Remove(other.gameObject);
        }
    }

    public void PreRotateFace()
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;
        // ResetCellBaseTransform();
        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis(5, Vector3.right), 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ReleaseObject();
            _state = State.PreRotated;
        });
    }

    public void ResetRotation()
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;

        // ResetCellBaseTransform();
        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ReleaseObject();
            _state = State.Base;
        });
    }

    public void RotateFace()
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;

        // ResetCellBaseTransform();
        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis(90, Vector3.right), 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ReleaseObject();
            // SetCellBaseTransform();
            _baseRotation = transform.localRotation;
            _state = State.Base;
        });
    }

    void ResetCellBaseTransform()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<CellController>().ResetTransform();
            }
        }
    }

    void SetCellBaseTransform()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<CellController>().SetBaseTransform();
            }
        }
    }

    void GrabObject()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = transform;
            }
        }
    }

    void ReleaseObject()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = transform.parent;
            }
        }
    }
}
