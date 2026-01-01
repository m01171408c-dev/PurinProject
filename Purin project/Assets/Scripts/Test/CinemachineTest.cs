using Unity.Cinemachine;
using UnityEngine;

public class CinemachineTest : MonoBehaviour
{
    [SerializeField] private Transform _virtualCamera;
    private Transform _followTarget = null;
    private Vector3 _distanceVec = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetTarget(Transform transform)
    {
        if (_virtualCamera == null)
        {
            Debug.LogError(this.name + " CinemachineCamera is not assigned.");
            return;
        }
        _followTarget = transform;
        _distanceVec = _virtualCamera.transform.position - _followTarget.position;
    }

    private void Update()
    {
        if (_followTarget == null) return;
        _virtualCamera.transform.position = _followTarget.position + _distanceVec;
    }
}
