using DG.Tweening;
using UnityEngine;

public class GrabberController : MonoBehaviour
{
    GameObject[] _grabbedObjects = new GameObject[9];
    Vector3 _forward;
    bool _isRotating;
    public bool IsRotating => _isRotating;

    void Start()
    {
        _forward = transform.right;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Grabber"))
        {
            for (int i = 0; i < _grabbedObjects.Length; i++)
            {
                if (_grabbedObjects[i] == null)
                {
                    _grabbedObjects[i] = other.gameObject;
                    break;
                }
            }
        }
    }

    public void RotateFace()
    {
        if (_isRotating) return;
        _isRotating = true;

        for (int i = 0; i < _grabbedObjects.Length; i++)
        {
            if (_grabbedObjects[i] != null)
            {
                _grabbedObjects[i].transform.parent = transform;
            }
        }

        transform.DOBlendableLocalRotateBy(_forward * 90, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            for (int i = 0; i < _grabbedObjects.Length; i++)
            {
                if (_grabbedObjects[i] != null)
                {
                    _grabbedObjects[i].transform.parent = transform.parent;
                }
            }
            _isRotating = false;
        });
    }
}
