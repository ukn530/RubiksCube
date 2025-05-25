using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrabberController : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler, IPointerDownHandler
{
    List<GameObject> _grabbedObjects = new List<GameObject>();
    [SerializeField] PlayController _playController;
    Quaternion _baseRotation;
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
        _baseRotation = transform.localRotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            _playController.ClickedGrabber(this);
        }
        _isClicking = false;
        _isDragging = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        _isDragging = _isClicking;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isClicking = true;
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
        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis(-3, Vector3.right), 0.05f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ReleaseObject();
            _state = State.PreRotated;
        });
    }

    public void ResetRotation()
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;

        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation, 0.05f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ReleaseObject();
            _state = State.Base;
        });
    }

    public void RotateFace(int rotation)
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;

        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis(-90 * (rotation + 1), Vector3.right), 0.1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            ReleaseObject();
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
