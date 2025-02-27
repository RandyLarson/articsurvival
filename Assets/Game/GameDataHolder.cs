using Unity.VisualScripting;
using UnityEngine;

public class GameDataHolder : MonoBehaviour, ISingleton
{
    [SerializeField] private GameState _gameState;
    [SerializeField] private GameClock _gameClock;
    [SerializeField] private GameConfig _gameConfig;
    public GameState GameState => _gameState;
    public GameConfig GameConfig => _gameConfig;
    public GameClock GameClock { get => _gameClock; set => _gameClock = value; }

    private static GameDataHolder _instance;
    public static GameDataHolder Current
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindFirstObjectByType<GameDataHolder>();
                //if (_instance == null)
                //{
                //    GameObject singleton = new GameObject(typeof(GameStateHolder).Name);
                //    _instance = singleton.AddComponent<GameStateHolder>();
                //    DontDestroyOnLoad(singleton);
                //}
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


}