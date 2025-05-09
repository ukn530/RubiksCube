using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GrabberController : MonoBehaviour
{
    List<GameObject> _grabbedObjects = new List<GameObject>();
    Vector3 _forward;
    bool _isRotating;
    public bool IsRotating => _isRotating;

    void Start()
    {
        _forward = transform.right;
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
