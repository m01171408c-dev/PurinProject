using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class GameMain : MonoBehaviour
{
    public enum GameState
    {
        None,
        Ready,
        Active,
        GameOver,
        GameClear
    }
    public static GameMain Instance { get; private set; }
    [SerializeField] private ObjectMaster _objectMaster = null;
    [SerializeField] private PlayerObject _playerObject = null;
    //[SerializeField] private GameObject _movieField = null;
    [SerializeField] private GameObject _gameField = null;
    //[SerializeField] private CinemachineCamera[] _readyMovieCameras = { };
    [SerializeField] private CinemachineCamera _playerCamera = null;
    [SerializeField] private PlayableDirector _movieDirector = null;
    [SerializeField] private PlayableAsset _gameReadyMovie = null;
    [SerializeField] private PlayableAsset _showBigMessageMovie = null;
    [SerializeField] private PlayableAsset _gameOverFallMovie = null;
    [SerializeField] private PlayableAsset _gameOverDeadMovie = null;
    [SerializeField] private TMPro.TextMeshProUGUI _timeText = null;
    [SerializeField] private TMPro.TextMeshProUGUI _bigMessageText = null;

    private GameState _state;
    private float _gameProgressTime = 0;
    private int _gameRemainTime = 0;
    private CancellationTokenSource _taskcancelToken;
    public ObjectMaster ObjectMaster => _objectMaster;
    public GameState State => _state;
    public float GameProgressTime => _gameProgressTime;
    public int GameRemainTime => _gameRemainTime;

    public const int PlayTime = 120; // å≈íËÇ≈Ç¶Ç¶Ç‚

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
        _taskcancelToken = new CancellationTokenSource();
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
                    _gameProgressTime += Time.deltaTime;
                    int remain = (int)(PlayTime - _gameProgressTime);
                    if (remain <= 0) { remain = 0; }
                    if(_gameRemainTime != remain)
                    {
                        _gameRemainTime = remain;
                        _timeText.text = _gameRemainTime.ToString();
                    }
                    _playerObject.PlayerUpdate();
                    _objectMaster.MasterUpdate();
                    CheckGameResult();
                }
                break;
            case GameState.GameOver:
                {
                    _playerObject.PlayerUpdate();
                }
                break;
            case GameState.GameClear:
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _taskcancelToken.Cancel();
    }

    private void ChangeGameState(GameState nextState)
    {
        _state = nextState;
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
                    _playerCamera.Priority = 0;
                    _bigMessageText.text = "GAME OVER!";
                    StartGameOverMoive();
                }
                break;
            case GameState.GameClear:
                {

                }
                break;
            default:
                break;
        }
        
    }

    private async UniTask ReadyMovieTask()
    {
        _gameField.SetActive(false);
        _movieDirector.playableAsset = _gameReadyMovie;
        _movieDirector.Play();
        await UniTask.WaitUntil(() => _movieDirector.state != PlayState.Playing, cancellationToken: _taskcancelToken.Token);
        Debug.Log("Title Main Opening Finish.");
        _gameField.SetActive(true);
        _playerCamera.Priority = 1;
        
        _playerObject.Init();
        await UniTask.Delay(1000, cancellationToken: _taskcancelToken.Token);
        _movieDirector.playableAsset = _showBigMessageMovie;
        _movieDirector.Play();
        await UniTask.WaitUntil(() => _movieDirector.state != PlayState.Playing, cancellationToken: _taskcancelToken.Token);
        _bigMessageText.gameObject.SetActive(false);
        _timeText.gameObject.SetActive(true);
        _timeText.text = PlayTime.ToString();
        _gameRemainTime = PlayTime;
        await UniTask.Delay(500, cancellationToken: _taskcancelToken.Token);
    }

    private async UniTask GameOverMovieTask()
    {
        if (_playerObject.GetMyPurin)
        {
            if (_playerObject.GetMyPurin.GetIsLanding() == false)
            {
                _movieDirector.playableAsset = _gameOverFallMovie;
                Debug.Log("óéâ∫éÄñS");
            }
            if (_playerObject.GetMyPurin.CurrentLife <= 0)
            {
                _movieDirector.playableAsset = _gameOverDeadMovie;
                Debug.Log("è’ìÀéÄñS");
            }
            _movieDirector.Play();
            await UniTask.WaitUntil(() => _movieDirector.state != PlayState.Playing, cancellationToken: _taskcancelToken.Token);
            await UniTask.Delay(500, cancellationToken: _taskcancelToken.Token);
        }
        await SceneManager.LoadSceneAsync(GameSceneName.Title).ToUniTask();
    }

    private async void StartReadyMovie()
    {
        await ReadyMovieTask();
        ChangeGameState(GameState.Active);
    }

    private async void StartGameOverMoive()
    {
        await GameOverMovieTask();
        ChangeGameState(GameState.None);
    }

    private void CheckGameResult()
    {
        if(_playerObject.GetMyPurin == null)
        {
            Debug.LogError("CheckGameResult GetMyPurin is not assin.");
            return;
        }
        if (_playerObject.GetMyPurin.CurrentState != Purin.PurinState.Dead)
        {
            if(_gameRemainTime <= 0)
            {
                ChangeGameState(GameState.GameClear);
            }
            return;
        }
        ChangeGameState(GameState.GameOver);
    }
}