using Unity.Cinemachine;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private MainGameInput _input = null;
    [SerializeField] private GameObject _purinPrefab;
    [SerializeField] private PurinConfig _purinConfig = null;
    [SerializeField] private CinemachineCamera _virtualCamera = null; // ムービー用カメラとの切り替えのためCinemachineCamera指定
    [SerializeField] private Transform _playerLifes = null; // UI回りは適当
    [SerializeField] private GameObject _myIcon = null;
    [SerializeField] private GameObject _directionMarker = null;
    private Purin _myPurin = null;
    private bool _isEnabled = false;
    private Vector3 _cameraDistanceVec = Vector3.zero;
    private int _lifeChecker = 0;

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
        if (_virtualCamera == null) 
        {
            Debug.LogError(this.name + " CinemachineCamera is not assigned.");
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
        _cameraDistanceVec = _virtualCamera.transform.position - _myPurin.transform.position;
        _lifeChecker = _myPurin.CurrentLife;
        
    }

    public void EnableInput()
    {
        if (_playerLifes != null)
        {
            // ライフUIを可視化
            _playerLifes.gameObject.SetActive(true);
        }
        if (_myIcon != null)
        {
            _myIcon.gameObject.SetActive(true);
        }
        if (_directionMarker == null)
        {
            Debug.LogError(this.name + " _directionMarker is not assigned.");
            return;
        }
        _directionMarker.SetActive(true);
        _directionMarker.transform.parent = _myPurin.transform;
        _directionMarker.transform.localPosition = Vector3.zero;
        _directionMarker.transform.localEulerAngles = Vector3.zero;

        _input.Enable();
        _isEnabled = true;
        _myPurin.StartMoving(); //仮。多分別の箇所で呼ぶ
    }

    public void DisableInput()
    {
        if (_playerLifes != null)
        {
            _playerLifes.gameObject.SetActive(false);
        }
        if (_myIcon != null)
        {
            _myIcon.gameObject.SetActive(false);
        }
        _directionMarker.SetActive(false);

        _input.Disable();
        _isEnabled = false;
    }

    public void PlayerUpdate()
    {
        if(_virtualCamera != null && _myPurin != null)
        {
            // 操作の有効無効に関わらずカメラは追従させる
            _virtualCamera.transform.position = _myPurin.transform.position + _cameraDistanceVec;
        }
        
        if (_myPurin != null)
        {
            _myPurin.StateUpdate();
            if (!_isEnabled)
            {
                return;
            }
            if (_myPurin.CurrentLife < _lifeChecker)
            {
                // ライフの回復に対応してない
                for (int i = 0; i < (_lifeChecker - _myPurin.CurrentLife) ; i++)
                {
                    Debug.Log(this.name + " life Reduced UI");
                    LifeReduce();
                }
                _lifeChecker = _myPurin.CurrentLife;
            }
            _myPurin.SetHandleMovement(_input.Purin.Handle.ReadValue<float>());
            if(_input.Purin.CreamAttack.WasPressedThisFrame() && _myPurin.IsCanCreamAttack())
            {
                _myPurin.StartCreamAttack();
            }
        }
    }

    private void LifeReduce()
    {
        if (_playerLifes == null) return;
        if(_lifeChecker <= 0)
        {
            return;
        }
        if(_playerLifes.childCount < _lifeChecker)
        {
            Debug.LogError("LifeReduce but _playerLifes.childCount is " + _playerLifes.childCount.ToString() + " and Life is " + _lifeChecker.ToString());
        }
        var index = _lifeChecker - 1;
        _playerLifes.GetChild(index).gameObject.SetActive(false);
    }
}
