using UnityEngine;

/// Central hookup from gameplay events (coin pickup, hit, death) to their
/// visual feedback (particle bursts, camera shake).
public class EffectsController : MonoBehaviour
{
    [SerializeField] private GameObject coinPopPrefab;
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private CameraShake cameraShake;

    private void OnEnable()
    {
        Coin.Collected += OnCoinCollected;
        PlayerHealth.Hit += OnPlayerHit;
        PlayerHealth.Died += OnPlayerDied;
    }

    private void OnDisable()
    {
        Coin.Collected -= OnCoinCollected;
        PlayerHealth.Hit -= OnPlayerHit;
        PlayerHealth.Died -= OnPlayerDied;
    }

    private void OnCoinCollected(Vector3 position)
    {
        if (coinPopPrefab != null)
        {
            Instantiate(coinPopPrefab, position, Quaternion.identity);
        }
    }

    private void OnPlayerHit(Vector3 position)
    {
        if (hitImpactPrefab != null)
        {
            Instantiate(hitImpactPrefab, position, Quaternion.identity);
        }
        if (cameraShake != null)
        {
            cameraShake.Shake(0.2f, 0.12f);
        }
    }

    private void OnPlayerDied()
    {
        if (cameraShake != null)
        {
            cameraShake.Shake(0.35f, 0.2f);
        }
    }
}
