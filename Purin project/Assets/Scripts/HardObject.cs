using UnityEngine;

public class HardObject : MonoBehaviour
{
    private float _minSpeed = 1.0f;
    private float _maxSpeed = 5.0f;
    private float _acceleration = 1.0f;

    private bool _isAction = false;
    private bool _isDestroyed = false;
    private float _currentSpeed = 0.0f;

    [SerializeField] private GameObject _model;

    public float CurrentSpeed => _currentSpeed;
    public bool IsDestroyed => _isDestroyed;
    public void StartAction()
    {
        _isAction = true;
    }

    public void ActionUpdate()
    {
        if (!_isAction) return;
        var prePosition = transform.position;
        var direction = transform.forward; // 仮。初期方向を外部から渡す
        if (_currentSpeed < _minSpeed)
        {
            _currentSpeed = _minSpeed;
        }
        else
        {
            _currentSpeed += Time.deltaTime * _acceleration; // 仮。加速値を外部から渡す
            if (_currentSpeed > _maxSpeed)
            {
                _currentSpeed = _maxSpeed;
            }
        }

        transform.position += direction * _currentSpeed * Time.deltaTime;
        var posDelta = transform.position - prePosition;
        var rotateSpeed = posDelta.magnitude / Time.deltaTime;
        var axis = Vector3.Cross(Vector3.up, posDelta.normalized);
        var radius = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f * 0.5f;
        var angle = (rotateSpeed / radius) * Mathf.Rad2Deg * Time.deltaTime;
        var rot = Quaternion.AngleAxis(angle, axis) * _model.transform.rotation;
        _model.transform.rotation = rot;
    }

    public void StopAndDestroy()
    {
        _isAction = false;
        _isDestroyed = true;
    }
}
