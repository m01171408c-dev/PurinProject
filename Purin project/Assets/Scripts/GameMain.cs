using UnityEngine;

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
#if true // 仮。ゲーム開始準備を飛ばす
        ChangeGameState(GameState.Active);
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
                break;
            case GameState.Active:
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
        _state = nextState;
    }
}