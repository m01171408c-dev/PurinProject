using Unity.Cinemachine;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private MainGameInput _input = null;
    [SerializeField] private GameObject _purinPrefab;
    [SerializeField] private PurinConfig _purinConfig = null;
    [SerializeField] private CinemachineCamera _virtualCamera = null; // ムービー用カメラとの切り替えのためCinemachineCamera指定
    private Purin _myPurin = null;
    private bool _isEnabled = false;
    private Vector3 _cameraDistanceVec = Vector3.zero;

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

    }

    public void EnableInput()
    {
        _input.Enable();
        _isEnabled = true;
        _myPurin.StartMoving(); //仮。多分別の箇所で呼ぶ
    }

    public void DisableInput()
    {
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
        if (!_isEnabled)
        {
            return;
        }

        if (_myPurin != null)
        {
            _myPurin.SetHandleMovement(_input.Purin.Handle.ReadValue<float>());
            _myPurin.StateUpdate();
        }
    }
}
