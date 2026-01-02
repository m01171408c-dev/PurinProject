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
    private string[] _targetListenTags = { GameTag.Enemy, GameTag.HardObject, GameTag.CreamItem, GameTag.CreamAttack };
    private CollisionListener _collisionListener = null;
    private bool _hasCream = false;
    private bool _isCreamAttackActive = false;
    private float _creamAttackTimer = 0f;
    private string[] _creamAttackListenTags = { GameTag.Enemy, GameTag.HardObject };
    private CollisionListener _creamAttackCollisionListener = null;
    private int _currentLife = 0;
    private float _notLandingTime = 0;
    private bool _isFall = false;
    public float CurrentSpeed => _currentSpeed;
    public PurinState CurrentState => _currentState;
    public int CurrentLife => _currentLife;


    [SerializeField] private Softbody _purinModel;
    [SerializeField] private SoftbodyProperty _purinModelProperty;
    [SerializeField] private Transform _creamParent;
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

    public bool IsCanCreamAttack()
    {
        if (_currentState != PurinState.Moving)
        {
            return false;
        }
        if (!_hasCream)
        {
            return false;
        }
        if (_isCreamAttackActive)
        {
            return false;
        }
        return true;
    }
    public void StartCreamAttack()
    {
        if (!IsCanCreamAttack())
        {
            Debug.LogWarning(this.name + " Cannot perform cream attack now.");
            return;
        }
        if (_creamAttackCollison == null)
        {
            Debug.LogError(this.name + " CreamAttackCollision prefab is not assigned.");
            return;
        }
        if (_purinConfig == null || _purinConfig.CreamAttackPrefab == null)
        {
            Debug.LogError(this.name + " PurinConfig is null in CreamAttack");
            return;
        }
        var creamAttack = GameObject.Instantiate(_purinConfig.CreamAttackPrefab, transform.position, _purinConfig.CreamAttackPrefab.transform.rotation);
        creamAttack.transform.parent = this.transform; creamAttack.transform.localPosition = _creamParent.localPosition;
        creamAttack.transform.localScale = Vector3.one * (_purinConfig.CreamAttackPrefabSize);
        var particles = creamAttack.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startLifetime = _purinConfig.AttackTime;
            particle.Play();
        }
        Destroy(creamAttack, _purinConfig.AttackTime + 0.1f);
        _creamAttackCollison.SetActive(true);
        _isCreamAttackActive = true;
        CreamsInvisible();
    }

    private void CreamAttckUpdate()
    {
        if (_isCreamAttackActive == false)
        {
            return;
        }
        _creamAttackTimer += Time.deltaTime;
        if (_creamAttackTimer >= _purinConfig.AttackTime)
        {
            _creamAttackCollison.SetActive(false);
            _isCreamAttackActive = false;
            _creamAttackTimer = 0f;
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
        CreamAttckUpdate();
        switch (_currentState)
        {
            case PurinState.Standby:
                // Do nothing
                break;
            case PurinState.Moving:
                {
                    HandleMovement();
                    CheckDead();
                }
                break;
            case PurinState.Knockback:
                {
                    KnockbackUpdate();
                    CheckDead();
                }
                break;
            case PurinState.Down:
                {
                    DownUpdate();
                    CheckDead();
                }
                break;
            case PurinState.Dead:
                {
                    FallUpdate();
                }
                break;
            default:
                Debug.LogError(this.name + " Invalid PurinState: " + _currentState);
                break;
        }
    }

    private void ResetSpeedStatus()
    {
        _currentSpeed = 0f;
        _knockbackTimer = 0f;
        _knockbackDirection = Vector3.zero;
        _handleHorizontal = 0f;
        _downTimer = 0f;
        _purinModel?.ResetAnimation();
    }

    private void CheckDead()
    {
        if (GetIsLanding() == false)
        {
            _notLandingTime += Time.deltaTime;
        }
        else
        {
            _notLandingTime = 0;
        }
        if (_notLandingTime > 0.5f)
        {
            // 0.5•b‚Ì—P—\
            _isFall = true;
            Die();
            return;
        }

        if (_currentLife <= 0)
        {
            Die();
            _purinModel.gameObject.SetActive(false);
            if (_purinConfig.DeadEffectPrefab != null)
            {
                var deadEffect = GameObject.Instantiate(_purinConfig.DeadEffectPrefab,
                    transform.position + new Vector3(0, 1, 0), _purinConfig.DeadEffectPrefab.transform.rotation);
                Destroy(deadEffect, 2f);
            }
        }
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
                ResetSpeedStatus();
                break;
            case PurinState.Moving:
                ResetSpeedStatus();
                {
                    _purinModel?.SetMoving(true);
                }
                break;
            case PurinState.Knockback:
                {
                    _purinModel?.Hit();
                    if (_purinConfig.DamageEffectPrefab != null)
                    {
                        var damageEffect = GameObject.Instantiate(_purinConfig.DamageEffectPrefab,
                            transform.position + new Vector3(0, 1, 0), _purinConfig.DamageEffectPrefab.transform.rotation);
                        Destroy(damageEffect, 2f);
                    }
                }
                break;
            case PurinState.Down:
                ResetSpeedStatus();
                break;
            case PurinState.Dead:
                ResetSpeedStatus();
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
                    if (_isCreamAttackActive)
                    {
                        break;
                    }
                    Debug.Log(this.name + " OnCollide with " + tag);
                    Vector3 knockbackDir = (transform.position - target.transform.position).normalized;
                    KnockBack(ref knockbackDir);
                    _currentLife--;
                }
                break;
            case GameTag.CreamItem:
                {
                    Debug.Log(this.name + " OnCollide with " + tag);
                    if (_hasCream)
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
                        if (creamModelIndex < 0)
                        {
                            break;
                        }
                        ShowMyCream(creamModelIndex);
                    }
                }
                break;
            case GameTag.CreamAttack:
                {
                    bool isMyAttack = false;
                    if (target.gameObject == _creamAttackCollison)
                    {
                        isMyAttack = true;
                    }
                    Debug.Log(this.name + " OnCollide with " + tag + (isMyAttack ? " My Attack" : " Enemy Attack"));
                }
                break;
            default:
                Debug.LogWarning(this.name + " OnCollide with unknown tag: " + tag);
                break;
        }
    }

    private void CreamAttackOnCollide(string tag, Collider target)
    {
        switch (tag)
        {
            case GameTag.Enemy:
                {
                    Debug.Log(this.name + " CreamAttack OnCollide with " + tag);
                }
                break;
            case GameTag.HardObject:
                {
                    Debug.Log(this.name + " CreamAttack OnCollide with " + tag);
                    if (target.transform.parent.TryGetComponent<HardObject>(out var hardObj))
                    {
                        hardObj.StopAndDestroy();
                        Debug.Log(this.name + " HardObject destroyed by CreamAttack.");
                    }
                }
                break;
            default:
                Debug.LogWarning(this.name + " CreamAttack OnCollide with unknown tag: " + tag);
                break;
        }
    }

    private void FallUpdate()
    {
        if (_isFall == false)
        {
            return;
        }
        var fallForce = Vector3.forward * _purinConfig.MaxSpeed + new Vector3(0, -9.8f, 0);
        transform.Translate(fallForce * Time.deltaTime);
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
        if (_creamAttackCollison == null)
        {
            Debug.LogError(this.name + " CreamAttackCollision is not assigned.");
            return false;
        }
        _currentLife = _purinConfig.Life;
        _creamAttackCollison.SetActive(false);
        var attackCol = _creamAttackCollison.GetComponent<SphereCollider>();
        if (attackCol != null)
        {
            attackCol.radius = _purinConfig.AttackRaduis;
        }
        else
        {
            Debug.LogError(this.name + " _creamAttackCollison Component is not valid");
        }
        if (_purinConfig.SpawnEffectPrefab != null)
        {
            var spawnEffect = GameObject.Instantiate(_purinConfig.SpawnEffectPrefab, transform.position + new Vector3(0, 1, 0), _purinConfig.SpawnEffectPrefab.transform.rotation);
            Destroy(spawnEffect, 2f);
        }
        _purinModel.SetUp(ref _purinModelProperty);
        _collisionListener = gameObject.AddComponent<CollisionListener>();
        _collisionListener.SetUp(ref _targetListenTags, OnCollide);
        _creamAttackCollisionListener = _creamAttackCollison.AddComponent<CollisionListener>();
        _creamAttackCollisionListener.SetUp(ref _creamAttackListenTags, CreamAttackOnCollide);
        ResetSpeedStatus();
        ChangeState(PurinState.Standby);
        return true;
    }

    public void StartMoving()
    {
        if (_currentState == PurinState.Moving)
        {
            return;
        }
        ResetSpeedStatus();
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
        ResetSpeedStatus();
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
        Debug.Log(this.name + " Purin is Dead");
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
