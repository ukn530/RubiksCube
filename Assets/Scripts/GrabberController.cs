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
    bool _isPC;

    public enum State
    {
        Base,
        PreRotatedR,
        PreRotatedL,
        Rotating,
    }

    State _state = State.Base;
    public State CurrentState => _state;

    void Start()
    {
        CheckScreenRatio();
        _baseRotation = transform.localRotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // if (!_isPC) return;
        if (!_isDragging)
        {
            _playController.ClickedGrabber(this);
        }
        _isClicking = false;
        _isDragging = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // if (!_isPC) return;
        _isDragging = _isClicking;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (!_isPC) return;
        _isClicking = true;
    }

    void Update()
    {
        CheckScreenRatio();
        //     if (!_isPC)
        //     {
        //         if (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Began))
        //         {
        //             _isClicking = true;
        //         }
        //         else if (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Moved))
        //         {
        //             _isDragging = _isClicking;
        //         }
        //         else if (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Ended))
        //         {
        //             Touch touch = Input.GetTouch(0);
        //             Ray ray = Camera.main.ScreenPointToRay(touch.position);
        //             RaycastHit hit;
        //             if (Physics.Raycast(ray, out hit))
        //             {
        //                 if (!_isDragging && hit.transform == transform)
        //                 {
        //                     _playController.ClickedGrabber(this);
        //                 }
        //             }
        //             _isClicking = false;
        //             _isDragging = false;
        //         }
        //     }
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

    public void PreRotateFace(bool isRight)
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;
        GrabObject();
        if (_isPC)
        {
            transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis(isRight ? -3 : 3, Vector3.right), 0.05f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                ReleaseObject();
                if (isRight) _state = State.PreRotatedR;
                else _state = State.PreRotatedL;
            });
        }
        else
        {
            if (_state == State.Rotating) return;
            if (isRight) _state = State.PreRotatedR;
            else _state = State.PreRotatedL;
        }
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

    public void RotateFace(int rotation, bool isRight)
    {
        if (_state == State.Rotating) return;
        _state = State.Rotating;

        GrabObject();

        transform.DOLocalRotateQuaternion(_baseRotation * Quaternion.AngleAxis((isRight ? -90 : 90) * (rotation + 1), Vector3.right), 0.1f).SetEase(Ease.InOutCubic).OnComplete(() =>
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

    void CheckScreenRatio()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio < 1 && _isPC)
        {
            _isPC = false; // Update the state to indicate that we are now in a mobile view
        }
        else if (ratio > 1 && !_isPC)
        {
            _isPC = true; // Update the state to indicate that we are now in a PC view
        }
    }
}
