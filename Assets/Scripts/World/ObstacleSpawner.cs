using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject groundObstaclePrefab;
    [SerializeField] private GameObject flyingObstaclePrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float minInterval = 1.2f;
    [SerializeField] private float maxInterval = 2.2f;
    [SerializeField] private float spawnBuffer = 2f;
    [SerializeField] private float groundObstacleChance = 0.4f;
    [SerializeField] private float flyingObstacleChance = 0.3f;

    private Camera mainCamera;
    private float timer;
    private float nextInterval;

    private void Awake()
    {
        mainCamera = Camera.main;
        PickNextInterval();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < nextInterval)
        {
            return;
        }

        timer = 0f;
        PickNextInterval();
        SpawnRandom();
    }

    private void PickNextInterval()
    {
        nextInterval = Random.Range(minInterval, maxInterval);
    }

    private void SpawnRandom()
    {
        float roll = Random.value;
        GameObject prefab;

        if (roll < groundObstacleChance)
        {
            prefab = groundObstaclePrefab;
        }
        else if (roll < groundObstacleChance + flyingObstacleChance)
        {
            prefab = flyingObstaclePrefab;
        }
        else
        {
            prefab = coinPrefab;
        }

        float spawnX = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect + spawnBuffer;
        float spawnY = prefab.transform.position.y;
        Instantiate(prefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
    }
}
