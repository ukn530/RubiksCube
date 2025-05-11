using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrabberController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    List<GameObject> _grabbedObjects = new List<GameObject>();
    Vector3 _forward;
    bool _isRotating;
    public bool IsRotating => _isRotating;

    bool _isClicking;
    bool _isDragging;

    void Start()
    {
        _forward = transform.right;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print($"オブジェクト {name} がクリックされたよ！");
        if (!_isDragging)
        {
            RotateFace();
        }
        _isClicking = false;
        _isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print($"オブジェクト {name} にマウスが乗ったよ！");
        // PreRotateFace();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print($"オブジェクト {name} からマウスが離れたよ！");
        // ResetRotation();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        print($"オブジェクト {name} にマウスが移動したよ！");
        // PreRotateFace();
        _isDragging = _isClicking;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print($"オブジェクト {name} がクリックされたよ！");
        _isClicking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print($"オブジェクト {name} のクリックが離れたよ！");
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

    void PreRotateFace()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = transform;
            }
        }

        transform.DOBlendableLocalRotateBy(_forward * 5, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            foreach (var grabbedObject in _grabbedObjects)
            {
                if (grabbedObject != null)
                {
                    grabbedObject.transform.parent = transform.parent;
                }
            }
        });
    }

    void ResetRotation()
    {
        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = transform;
            }
        }

        transform.DOBlendableLocalRotateBy(_forward * -5, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            foreach (var grabbedObject in _grabbedObjects)
            {
                if (grabbedObject != null)
                {
                    grabbedObject.transform.parent = transform.parent;
                }
            }
        });
    }

    public void RotateFace()
    {
        _isRotating = true;

        foreach (var grabbedObject in _grabbedObjects)
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = transform;
            }
        }

        transform.DOBlendableLocalRotateBy(_forward * 90, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            foreach (var grabbedObject in _grabbedObjects)
            {
                if (grabbedObject != null)
                {
                    grabbedObject.transform.parent = transform.parent;
                }
            }
            _isRotating = false;
        });
    }
}
