using UnityEngine;

public class GameplayManager : MonoBehaviourSingleton<GameplayManager>
{
    [SerializeField] private Transform _roadTransform;

    public float RoadSpeed { get; private set; } = 2f;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
            return true;
        }
        return false;
    }

    private void Update()
    {
        _roadTransform.position += Vector3.forward * Time.deltaTime * RoadSpeed;
        if (_roadTransform.position.z > 18f)
            _roadTransform.position -= new Vector3(0f, 0f, 18f);
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        
    }
}
