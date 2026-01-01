using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private MainGameInput _input = null;
    [SerializeField] private GameObject _purinPrefab;
    [SerializeField] private PurinConfig _purinConfig = null;
    private const string _playerTag = "Player";
    private Purin _myPurin = null;
    private bool _isEnabled = false;

    public Purin GetMyPurin => _myPurin;

    public void Init()
    {
        _input ??= new MainGameInput();
        if (_input == null)
        {
            Debug.LogError(this.name + " MainGameInput is not assigned.");
            return;
        }
        if (_purinPrefab == null)
        {
            Debug.LogError(this.name + " Purin Prefab is not assigned.");
            return;
        }
        if (_purinConfig == null)
        {
            Debug.LogError(this.name + " Purin Config is not assigned.");
            return;
        }
        _purinPrefab = GameObject.Instantiate(_purinPrefab, this.transform);
        _purinPrefab.transform.localPosition = Vector3.zero;
        _purinPrefab.transform.parent = this.transform;
        _myPurin = _purinPrefab.GetComponent<Purin>();
        if (_myPurin == null)
        {
            Debug.LogError(this.name + " Purin component is not found in Purin Prefab.");
            return;
        }
        _myPurin.SetUp(ref _purinConfig);
    }

    public void EnableInput()
    {
        _input.Enable();
        _isEnabled = true;
        _myPurin.StartMoving(); //âºÅBëΩï™ï ÇÃâ”èäÇ≈åƒÇ‘
    }

    public void DisableInput()
    {
        _input.Disable();
        _isEnabled = false;
    }

    public void PlayerUpdate()
    {
        if (!_isEnabled)
        {
            return;
        }
        //ColiisionUpdate();
        if (_myPurin != null)
        {
            _myPurin.SetHandleMovement(_input.Purin.Handle.ReadValue<float>());
            _myPurin.StateUpdate();
        }
    }

#if false // ìñÇΩÇËîªíËÇÕCollisionListenerÇ≈èàóùÇ∑ÇÈÇΩÇﬂïsóv
    private void CollideObject(Vector3 collidePosition)
    {
        if (_myPurin == null)
        {
            Debug.LogError(this.name + " Purin component is not assigned.");
            return;
        }
        Vector3 knockbackDirection = (_myPurin.transform.position - collidePosition).normalized;
        _myPurin.KnockBack(ref knockbackDirection);
    }
    private void ColiisionUpdate()
    {
        if (_myPurin == null)
        {
            Debug.LogError(this.name + " Purin component is not assigned.");
            return;
        }
        var center = _myPurin.transform.position + new Vector3(0, 0, 0);
        var halfExtents = new Vector3(2,2,2);
        Collider[] hits = Physics.OverlapBox(center, halfExtents, _myPurin.transform.rotation);
        bool isLanding = false;
        foreach (var hit in hits)
        {
            if (hit.tag == _playerTag)
            {
                continue;
            }
            switch (hit.tag)
            {
                case "Object":
                    Debug.Log(this.name + " hit Object: " + hit.name);
                    {
                        CollideObject(hit.transform.position);
                    }
                    break;
                case "Enemy":
                    Debug.Log(this.name + " hit Enemy: " + hit.name);
                    break;
                case "Floor":
                    isLanding = true;
                    break;
                default:
                    break;
            }
        }
        //_myPurin.SetIsLanding(isLanding);
    }
#endif
}
