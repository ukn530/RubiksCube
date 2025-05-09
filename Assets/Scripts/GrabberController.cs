using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GrabberController : MonoBehaviour
{
    GameObject[] _grabbedObjects = new GameObject[9];
    Vector3 _forward;
    bool _isRotating;
    public bool IsRotating => _isRotating;
    Vector3 _basePosition;

    void Start()
    {
        _forward = transform.right;
        _basePosition = transform.position;
        transform.position = _basePosition * 100;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cell"))
        {
            for (int i = 0; i < _grabbedObjects.Length; i++)
            {
                if (_grabbedObjects[i] == null)
                {
                    _grabbedObjects[i] = other.gameObject;
                    _grabbedObjects[i].transform.parent = transform;
                    break;
                }
            }

        }
    }

    public void RotateFace()
    {
        if (_isRotating) return;
        _isRotating = true;
        transform.position = _basePosition;
        StartCoroutine(RotateFaceCoroutine());
    }

    IEnumerator RotateFaceCoroutine()
    {
        yield return null;
        transform.DOBlendableLocalRotateBy(_forward * 90, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            for (int i = 0; i < _grabbedObjects.Length; i++)
            {
                if (_grabbedObjects[i] != null)
                {
                    _grabbedObjects[i].transform.parent = transform.parent;
                    _grabbedObjects[i] = null;
                }
            }
            transform.position = _basePosition * 100;
            _isRotating = false;
        });
    }
}
