using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeParamater : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] GameObject _target;

    void Update()
    {
        if (_volume != null && _volume.profile.TryGet<DepthOfField>(out var dof))
        {
            float distance = Vector3.Distance(_target.transform.position, Camera.main.transform.position);
            dof.focusDistance.value = distance;
        }
    }
}
