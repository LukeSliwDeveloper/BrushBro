using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviourSingleton<GameplayManager>
{
    [SerializeField] private GameObject _motorbikePrefab;
    [SerializeField] private DestroyedCaller _bumpPrefab;
    [SerializeField] private GameObject[] _obstaclePrefabs;
    [SerializeField] private GameObject[] _plantPrefabs;
    [SerializeField] private Transform _roadTransform;

    private float _nextObstacleSpawnDistanceLeft, _nextBumpSpawnDistanceLeft, _nextBikeSpawnDistanceLeft;
    private float _nextPlantSpawnDistanceLeft, _nextPlantSpawnDistanceRight;
    private bool _didPlayerTakeDamage;

    private Vector2 _obstacleSpawnInterval = new Vector2(2f, 6f);
    private Vector2 _bumpSpawnInterval = new Vector2(12f, 18f);
    private Vector2 _bikeSpawnInterval = new Vector2(20f, 25f);
    private Vector2 _plantSpawnInterval = new Vector2(2.5f, 5f);
    private int _playerLives = 3;

    public float RoadSpeed { get; private set; } = 2f;

    public event Action OnMove;
    public event Action<int> OnPlayerHit;
    public event Action<bool> OnBumpTutorialComplete;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            DontDestroyOnLoad(gameObject);
            GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
            _nextPlantSpawnDistanceLeft = 0.5f;
            _nextPlantSpawnDistanceRight = 1.5f;
            return true;
        }
        return false;
    }

    private void Update()
    {
        GameplayTick();
        _roadTransform.position += Vector3.forward * Time.deltaTime * RoadSpeed;
        if (_roadTransform.position.z > 18f)
            _roadTransform.position -= new Vector3(0f, 0f, 18f);
        if ((_nextPlantSpawnDistanceLeft -= Time.deltaTime * RoadSpeed) <= 0f)
        {
            _nextPlantSpawnDistanceLeft += Random.Range(_plantSpawnInterval.x, _plantSpawnInterval.y);
            Instantiate(_plantPrefabs[Random.Range(0, _plantPrefabs.Length)], 
                new Vector3(Random.Range(-7.4f, -6.2f), 0f, -5.4f), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        }
        if ((_nextPlantSpawnDistanceRight -= Time.deltaTime * RoadSpeed) <= 0f)
        {
            _nextPlantSpawnDistanceRight += Random.Range(_plantSpawnInterval.x, _plantSpawnInterval.y);
            Instantiate(_plantPrefabs[Random.Range(0, _plantPrefabs.Length)],
                new Vector3(Random.Range(6.2f, 7.4f), 0f, -5.4f), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        }
    }

    public bool RevivePlayer()
    {
        OnPlayerHit?.Invoke(--_playerLives);
        _didPlayerTakeDamage = true;
        if (_playerLives <= 0)
        {
            if (_playerLives == 0)
                GameManager.Instance.ChangeGameState(GameState.GameOver);
            return false;
        }
        return true;
    }

    private void GameplayTick()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            RoadSpeed += Time.deltaTime * 0.05f;
            if (RoadSpeed > 10f)
                RoadSpeed = 10f;

            if ((_nextBumpSpawnDistanceLeft -= Time.deltaTime * RoadSpeed) <= 0f)
            {
                var bump = Instantiate(_bumpPrefab, new Vector3(0f, 0f, -5.4f), Quaternion.identity);
                if (!GameManager.Instance.WasBumpTutorialComplete)
                {
                    bump.OnDestroyed += CheckBumpTutorialComplete;
                    OnBumpTutorialComplete?.Invoke(false);
                    _didPlayerTakeDamage = false;
                }
                _nextBumpSpawnDistanceLeft = Random.Range(_bumpSpawnInterval.x, _bumpSpawnInterval.y);
            }
            if ((_nextObstacleSpawnDistanceLeft -= Time.deltaTime * RoadSpeed) <= 0f)
            {
                SpawnObstacle(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)]);
                _nextObstacleSpawnDistanceLeft = Random.Range(_obstacleSpawnInterval.x, _obstacleSpawnInterval.y);
            }
            if ((_nextBikeSpawnDistanceLeft -= Time.deltaTime * RoadSpeed) <= 0f)
            {
                int offset = Random.Range(0, 15);
                int xIndex;
                for (int i = 0; i < 15; i++)
                {
                    xIndex = (i + offset) % 15 - 7;
                    if (Physics.BoxCast(new Vector3(.5f * xIndex, .5f, -5.4f), new Vector3(.14f, .1f, .1f), Vector3.forward, out var hit)
                        && hit.collider.CompareTag("Finish")) ;
                    else
                    {
                        Instantiate(_motorbikePrefab, new Vector3(.5f * xIndex, 0f, -5.4f), Quaternion.identity);
                        break;
                    }
                }
                _nextBikeSpawnDistanceLeft = Random.Range(_bikeSpawnInterval.x, _bikeSpawnInterval.y);
            }
            OnMove?.Invoke();
        }
    }

    private void CheckBumpTutorialComplete()
    {
        if (!_didPlayerTakeDamage)
        {
            GameManager.Instance.CompleteBumpTutorial();
            OnBumpTutorialComplete?.Invoke(true);
        }
    }

    private void SpawnObstacle(GameObject obstaclePrefab)
    {
        Vector3 spawnPos = new(Random.Range(-3.4f, 3.4f), 0f, -5.4f);
        Quaternion spawnRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Instantiate(obstaclePrefab, spawnPos, spawnRot);
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.Menu)
        {
            RoadSpeed = 2f;
            _playerLives = 3;
        }
        if (gameState == GameState.Gameplay)
        {
            _nextObstacleSpawnDistanceLeft = Random.Range(_obstacleSpawnInterval.x, _obstacleSpawnInterval.y);
            _nextBumpSpawnDistanceLeft = 25f;
            _nextBikeSpawnDistanceLeft = 100f;
        }
        if (gameState == GameState.GameOver)
        {
            RoadSpeed = 0f;
        }
    }
}
