using UnityEngine;

public class CellController : MonoBehaviour
{
    Quaternion _baseRotation = Quaternion.identity;
    Vector3 _basePosition = Vector3.zero;

    public void SetBaseTransform()
    {
        _baseRotation = transform.rotation;
        _basePosition = transform.position;
    }

    public void ResetTransform()
    {
        transform.rotation = _baseRotation;
        transform.position = _basePosition;
    }
}
