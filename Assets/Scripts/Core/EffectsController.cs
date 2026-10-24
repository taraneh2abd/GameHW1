using UnityEngine;

/// Central hookup from gameplay events (hit, death) to camera feedback.
public class EffectsController : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;

    private void OnEnable()
    {
        PlayerHealth.Hit += OnPlayerHit;
        PlayerHealth.Died += OnPlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.Hit -= OnPlayerHit;
        PlayerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerHit(Vector3 position)
    {
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
