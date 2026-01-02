using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;


public class GameMain : MonoBehaviour
{
    public enum GameState
    {
        None,
        Ready,
        Active,
        GameOver
    }
    public static GameMain Instance { get; private set; }
    [SerializeField] private ObjectMaster _objectMaster = null;
    [SerializeField] private PlayerObject _playerObject = null;
    [SerializeField] private GameObject _movieField = null;
    [SerializeField] private GameObject _gameField = null;
    [SerializeField] private CinemachineCamera[] _readyMovieCameras = { };
    [SerializeField] private CinemachineCamera _playerCamera = null;
    
    private GameState _state;
    private float _gameProgressTime = 0;
    public ObjectMaster ObjectMaster => _objectMaster;
    public GameState State => _state;
    public float GameProgressTime => _gameProgressTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if true // âºÅB
        ChangeGameState(GameState.Ready);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case GameState.None:
                break;
            case GameState.Ready:
                break;
            case GameState.Active:
                {
                    _playerObject.PlayerUpdate();
                    _objectMaster.MasterUpdate();
                }
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }

    private void ChangeGameState(GameState nextState)
    {

        switch (nextState)
        {
            case GameState.None:
                break;
            case GameState.Ready:
                {
                    StartReadyMovie();
                }
                break;
            case GameState.Active:
                {
                    _objectMaster.Setup();
                    _playerObject.EnableInput();
                }
                break;
            case GameState.GameOver:
                {
                    _playerObject.DisableInput();
                }
                break;
            default:
                break;
        }
        _state = nextState;
    }

    private async UniTask ReadyMovieTask()
    {
        if(_readyMovieCameras.Length < 2)
        {
            Debug.LogError(this.name + " _readyMovieCameras Length is invaild");
        }
        _gameField.SetActive(false);
        _movieField.SetActive(true);
        foreach(var cam in  _readyMovieCameras)
        {
            cam.Priority = 0;
        }
        _readyMovieCameras[0].Priority = 1;
        await UniTask.Delay(1000); // 1ïbë“Ç¬
        _readyMovieCameras[0].Priority = 0;
        _readyMovieCameras[1].Priority = 1;
        await UniTask.Delay(3000);
        _gameField.SetActive(true);
        _movieField.SetActive(false);
        _readyMovieCameras[1].Priority = 0;
        _playerCamera.Priority = 1;
        await UniTask.Delay(500);
        _playerObject.Init();
        await UniTask.Delay(1000);
    }

    private async void StartReadyMovie()
    {
        await ReadyMovieTask();
        ChangeGameState(GameState.Active);
    }
}