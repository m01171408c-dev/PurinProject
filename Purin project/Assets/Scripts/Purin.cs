using UnityEditor.SceneManagement;
using UnityEngine;

public class Purin : MonoBehaviour
{
    public enum PurinState
    {
        Standby,
        Moving,
        Knockback,
        Down,
        Dead
    }

    private PurinConfig _purinConfig = null;

    private float _currentSpeed = 0.0f;
    private float _knockbackTimer = 0f;
    private Vector3 _knockbackDirection = Vector3.zero;
    private float _downTimer = 0f;

    private float _handleHorizontal = 0f;
    private PurinState _currentState = PurinState.Standby;
    private string[] _targetListenTags = { GameTag.Enemy, GameTag.HardObject, GameTag.CreamItem,GameTag.CreamAttack };
    private CollisionListener _collisionListener = null;
    private bool _hasCream = false;
    public float CurrentSpeed => _currentSpeed;
    public PurinState CurrentState => _currentState;


    [SerializeField] private Softbody _purinModel;
    [SerializeField] private SoftbodyProperty _purinModelProperty;
    [SerializeField] private Transform _creamParent;

    [SerializeField] private GameObject _creamAttackPrefab;
    [SerializeField] private GameObject _creamAttackCollison;
    private void AutoAccelerate()
    {
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in AutoAccelerate");
            return;
        }
        if (_currentSpeed < _purinConfig.MaxSpeed)
        {
            _currentSpeed += _purinConfig.Acceleration;

        }
        else
        {
            _currentSpeed = _purinConfig.MaxSpeed;
        }
    }

    private void TurnDecelerate()
    {
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in TurnDecelerate");
            return;
        }
        if (_currentSpeed > _purinConfig.MinSpeed)
        {
            _currentSpeed -= _purinConfig.Deceleration * _purinConfig.BreakSpeed;
        }
        else
        {
            _currentSpeed = _purinConfig.MinSpeed;
        }
    }

    private void KnockbackUpdate()
    {
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in KnockbackUpdate");
            return;
        }
        if (_currentState != PurinState.Knockback)
        {
            return;
        }
        _knockbackTimer += Time.deltaTime;
        if (_knockbackTimer >= _purinConfig.KnockbackTime)
        {
            ShunDown();
            return;
        }
        float knockbackSpeed = _purinConfig.KnockbackDistance / _purinConfig.KnockbackTime;
        transform.Translate(_knockbackDirection * knockbackSpeed * Time.deltaTime, Space.World);
    }

    private void DownUpdate()
    {
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in DownUpdate");
            return;
        }
        if (_currentState != PurinState.Down)
        {
            return;
        }
        _downTimer += Time.deltaTime;
        if (_downTimer >= _purinConfig.DownTime)
        {
            StartMoving();
            return;
        }
    }

    private void HandleMovement()
    {
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in HandleMovement");
            return;
        }
        bool isTurn = (_handleHorizontal != 0);
        if (isTurn == false)
        {
            AutoAccelerate();
        }
        else
        {
            TurnDecelerate();
        }
        _currentSpeed = Mathf.Clamp(_currentSpeed, _purinConfig.MinSpeed, _purinConfig.MaxSpeed);
        if (isTurn)
        {
            float turnAmount = _handleHorizontal * _purinConfig.TurnSpeed * Time.deltaTime * (_currentSpeed / _purinConfig.MaxSpeed);
            Vector3 angles = transform.localEulerAngles; angles.y += turnAmount; transform.localEulerAngles = angles;

        }
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
    }

    private void CreamsInvisible()
    {
        if (_creamParent == null)
        {
            Debug.LogWarning(this.name + " CreamParent is not assigned.");
            return;
        }
        foreach (Transform cream in _creamParent)
        {
            cream.gameObject.SetActive(false);
        }
        _hasCream = false;
    }

    private void ShowMyCream(int index)
    {
        if (_creamParent.childCount <= index)
        {
            Debug.LogWarning(this.name + " Cream model index is out of range: " + index);
            return;
        }
        _creamParent.GetChild(index).gameObject.SetActive(true);
        _hasCream = true;
    }

    public void StateUpdate()
    {
        _purinModel?.AnimationUpdate();
        switch (_currentState)
        {
            case PurinState.Standby:
                // Do nothing
                break;
            case PurinState.Moving:
                HandleMovement();
                break;
            case PurinState.Knockback:
                KnockbackUpdate();
                break;
            case PurinState.Down:
                DownUpdate();
                break;
            case PurinState.Dead:
                // Do nothing
                break;
            default:
                Debug.LogError(this.name + " Invalid PurinState: " + _currentState);
                break;
        }
    }

    private void ResetStatus()
    {
        _currentSpeed = 0f;
        _knockbackTimer = 0f;
        _knockbackDirection = Vector3.zero;
        _handleHorizontal = 0f;
        _downTimer = 0f;
        _purinModel?.ResetAnimation();
    }

    private void ChangeState(PurinState nextState)
    {
        if (_currentState == nextState)
        {
            return;
        }
        _currentState = nextState;
        switch (nextState)
        {
            case PurinState.Standby:
                ResetStatus();
                break;
            case PurinState.Moving:
                ResetStatus();
                {
                    _purinModel?.SetMoving(true);
                }
                break;
            case PurinState.Knockback:
                {
                    _purinModel?.Hit();
                }
                break;
            case PurinState.Down:
                ResetStatus();
                break;
            case PurinState.Dead:
                ResetStatus();
                break;
            default:
                Debug.LogError(this.name + " Invalid PurinState: " + nextState);
                break;
        }
    }

    private void OnCollide(string tag, Collider target)
    {
        switch (tag)
        {
            case GameTag.Enemy:
            case GameTag.HardObject:
                {
                    Debug.Log(this.name + " OnCollide with " + tag);
                    Vector3 knockbackDir = (transform.position - target.transform.position).normalized;
                    KnockBack(ref knockbackDir);
                    // Todo:ƒ‰ƒCƒtŒ¸­ˆ—
                }
                break;
            case GameTag.CreamItem:
                {
                    Debug.Log(this.name + " OnCollide with " + tag);
                    if(_hasCream)
                    {
                        Debug.Log(this.name + " Already has cream. Cannot take more.");
                        break;
                    }
                    if (_creamParent == null)
                    {
                        Debug.LogWarning(this.name + " CreamParent is not assigned.");
                        break;
                    }
                    CreamsInvisible();
                    if (target.TryGetComponent<Cream>(out var cream))
                    {
                        int creamModelIndex = cream.PurinTakeCream();
                        ShowMyCream(creamModelIndex);
                    }
                }
                break;
            default:
                Debug.LogWarning(this.name + " OnCollide with unknown tag: " + tag);
                break;
        }
    }

    public bool SetUp(ref PurinConfig purinConfig)
    {
        if (purinConfig == null)
        {
            Debug.LogError(this.name + " PurinConfig is not assigned.");
            return false;
        }
        _purinConfig = purinConfig;
        if (_purinModel == null || _purinModelProperty == null)
        {
            Debug.LogError(this.name + " PurinModel is not assigned.");
            return false;
        }
        _purinModel.SetUp(ref _purinModelProperty);
        _collisionListener = gameObject.AddComponent<CollisionListener>();
        _collisionListener.SetUp(ref _targetListenTags, OnCollide);
        ResetStatus();
        ChangeState(PurinState.Standby);
        return true;
    }

    public void StartMoving()
    {
        if (_currentState == PurinState.Moving)
        {
            return;
        }
        ResetStatus();
        ChangeState(PurinState.Moving);
    }

    public bool SetHandleMovement(float horizontal)
    {
        if (_currentState != PurinState.Moving)
        {
            return false;
        }
        if (horizontal < -1f || horizontal > 1f)
        {
            Debug.LogWarning(this.name + " Handle horizontal input is out of range: " + horizontal);
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        }
        _handleHorizontal = horizontal;
        return true;
    }

    public void KnockBack(ref Vector3 knckbackDir)
    {
        if (_currentState == PurinState.Standby || _currentState == PurinState.Dead)
        {
            Debug.LogWarning(this.name + " Purin is in other state. Cannot knockback.");
            return;
        }
        ResetStatus();
        _knockbackDirection = knckbackDir.normalized;
        _knockbackDirection.y = 0;
        ChangeState(PurinState.Knockback);
    }

    public void ShunDown()
    {
        if (_currentState == PurinState.Standby || _currentState == PurinState.Dead)
        {
            Debug.LogError(this.name + " Purin is in other state. Cannot shutdown.");
        }
        _downTimer = 0f;
        ChangeState(PurinState.Down);
    }

    public void Die()
    {
        ChangeState(PurinState.Dead);
    }

    public bool GetIsLanding()
    {
        if (_collisionListener == null)
        {
            Debug.LogError(this.name + " CollisionListener is not assigned.");
            return false;
        }
        return _collisionListener.IsGrounded;
    }
}
