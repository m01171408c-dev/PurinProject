using UnityEngine;

public class HardObject : MonoBehaviour
{
    public enum HardObjectState
    {
        Inactive,
        Active,
        Shocked,
        Destroyed
    }
    private float _minSpeed = 1.0f;
    private float _maxSpeed = 5.0f;
    private float _acceleration = 1.0f;

    private float _currentSpeed = 0.0f;
    private HardObjectState _currentState = HardObjectState.Inactive;

    private Vector3 _direction = Vector3.forward;
    private GameObject _model = null;
    private float _shockTimer = 0f;
    private float _activeTimer = 0f;

    private string[] _targetListenTags = { GameTag.Enemy, GameTag.HardObject, GameTag.Player };
    private CollisionListener _collisionListener = null;

    public float CurrentSpeed => _currentSpeed;
    public HardObjectState CurrentState => _currentState;

    private void OnCollide(string tag, Collider target)
    {
        switch (tag)
        {
            case GameTag.Player:
            case GameTag.Enemy:
                {
                    Debug.Log(this.name + " OnCollide with Purin");
                    ShockAndDown();
                }
                break;
            case GameTag.HardObject:
                {
                    Debug.Log(this.name + " OnCollide with " + tag);
                    DirectionTurn();
                }
                break;
            default:
                Debug.LogWarning(this.name + " OnCollide with unknown tag: " + tag);
                break;
        }
    }
    public void StartAction(Vector3 direction)
    {
        if (_currentState != HardObjectState.Inactive)
        {
            Debug.LogWarning(this.name + " StartAction called but current state is not Inactive.");
            return;
        }
        _model = transform.GetChild(0).gameObject;
        if (_model == null)
        {
            Debug.LogError(this.name + " Model is not found.");
            return;
        }
        _collisionListener = _model.AddComponent<CollisionListener>();
        _collisionListener.SetUp(ref _targetListenTags, OnCollide);
        _direction = direction.normalized;
        _currentState = HardObjectState.Active;
    }

    public void ActionUpdate()
    {
        if (_currentState == HardObjectState.Shocked)
        {
            _shockTimer += Time.deltaTime;
            if (_shockTimer >= 3.0f) // 仮。ショック状態の継続時間を外部から渡す
            {
                StopAndDestroy();
            }
            return;
        }
        if (_currentState != HardObjectState.Active)
        {
            return;
        }
        if (_model == null)
        {
            Debug.LogError(this.name + " Model is not found.");
            return;
        }
        _activeTimer += Time.deltaTime;
        if (_activeTimer >= 1 && _collisionListener.IsGrounded == false)
        {
            StopAndDestroy(); //場外のため削除
            return;
        }
        var prePosition = transform.position;
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

        transform.position += _direction * _currentSpeed * Time.deltaTime;
        var posDelta = transform.position - prePosition;
        var rotateSpeed = posDelta.magnitude / Time.deltaTime;
        var axis = Vector3.Cross(Vector3.up, posDelta.normalized);
        var radius = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f * 0.5f;
        var angle = (rotateSpeed / radius) * Mathf.Rad2Deg * Time.deltaTime;
        var rot = Quaternion.AngleAxis(angle, axis) * _model.transform.rotation;
        _model.transform.rotation = rot;
    }

    public void ShockAndDown()
    {
        if (_currentState != HardObjectState.Active)
        {
            return;
        }
        _shockTimer = 0;
        _currentState = HardObjectState.Shocked;
    }

    public void StopAndDestroy()
    {
        _shockTimer = 0;
        _currentState = HardObjectState.Destroyed;
    }

    public void DirectionTurn()
    {
        if (_currentState != HardObjectState.Active && _currentState != HardObjectState.Shocked)
        {
            return;
        }
        _direction = -_direction;
    }
}
