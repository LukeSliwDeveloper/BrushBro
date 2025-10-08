using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviourSingleton<GameplayManager>
{
    [SerializeField] private GameObject _conePrefab;
    [SerializeField] private Transform _roadTransform;

    private float _nextConeSpawnTime;
    private float _nextMotorcycleSpawnTime;

    private Vector2 _coneSpawnInterval = new Vector2(0.5f, 1.5f);
    private Vector2 _motorcycleSpawnInterval = new Vector2(1f, 2.5f);
    private int _playerLives = 3;

    public float RoadSpeed { get; private set; } = 2f;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            DontDestroyOnLoad(gameObject);
            GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
            return true;
        }
        return false;
    }

    private void Update()
    {
        GameplayTick();
    }

    private void FixedUpdate()
    {
        _roadTransform.position += Vector3.forward * Time.fixedDeltaTime * RoadSpeed;
        if (_roadTransform.position.z > 18f)
            _roadTransform.position -= new Vector3(0f, 0f, 18f);
    }

    public bool RevivePlayer()
    {
        if (--_playerLives <= 0)
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

            if (Time.time > _nextConeSpawnTime)
            {
                SpawnObstacle(_conePrefab);
                _nextConeSpawnTime = Time.time + UnityEngine.Random.Range(_coneSpawnInterval.x, _coneSpawnInterval.y) / (RoadSpeed / 2f);
            }
        }
    }

    private void SpawnObstacle(GameObject obstaclePrefab)
    {
        Vector3 spawnPos = new(Random.Range(-3.8f, 3.8f), 0f, -5.4f);
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
            _nextConeSpawnTime = Time.time + 3f;
            _nextMotorcycleSpawnTime = Time.time + 20f;
        }
        if (gameState == GameState.GameOver)
        {
            RoadSpeed = 0f;
        }
    }
}
