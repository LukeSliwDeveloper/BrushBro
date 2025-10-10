using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviourSingleton<GameplayManager>
{
    [SerializeField] private GameObject _bumpPrefab;
    [SerializeField] private GameObject[] _obstaclePrefabs;
    [SerializeField] private GameObject[] _plantPrefabs;
    [SerializeField] private Transform _roadTransform;

    private float _nextObstacleSpawnDistanceLeft, _nextBumpSpawnDistanceLeft;
    private float _nextMotorcycleSpawnTime;
    private float _nextPlantSpawnDistanceLeft, _nextPlantSpawnDistanceRight;

    private Vector2 _obstacleSpawnInterval = new Vector2(2f, 6f);
    private Vector2 _bumpSpawnInterval = new Vector2(8f, 12f);
    private Vector2 _motorcycleSpawnInterval = new Vector2(1f, 2.5f);
    private Vector2 _plantSpawnInterval = new Vector2(2.5f, 5f);
    private int _playerLives = 3;

    public float RoadSpeed { get; private set; } = 2f;

    public event Action OnMove;

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
        if (--_playerLives == 0)
        {
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
                Instantiate(_bumpPrefab, new Vector3(Random.Range(-2.1f, 2.1f), 0f, -5.4f), Quaternion.identity);
                _nextBumpSpawnDistanceLeft = Random.Range(_bumpSpawnInterval.x, _bumpSpawnInterval.y);
            }
            if ((_nextObstacleSpawnDistanceLeft -= Time.deltaTime * RoadSpeed) <= 0f)
            {
                SpawnObstacle(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)]);
                _nextObstacleSpawnDistanceLeft = Random.Range(_obstacleSpawnInterval.x, _obstacleSpawnInterval.y);
            }
            OnMove?.Invoke();
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
            _nextBumpSpawnDistanceLeft = Random.Range(_bumpSpawnInterval.x, _bumpSpawnInterval.y);
            _nextMotorcycleSpawnTime = Time.time + 20f;
        }
        if (gameState == GameState.GameOver)
        {
            RoadSpeed = 0f;
        }
    }
}
