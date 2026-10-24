using UnityEngine;

/// Common base for anything that scrolls toward the player and reacts on
/// contact (obstacles, score items). Handles the shared scrolling/despawn
/// behavior; subclasses only decide what happens on contact.
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private float speedScale = 1f;
    [SerializeField] private float despawnBuffer = 2f;

    private Camera mainCamera;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        transform.position += Vector3.left * (GameSpeed.Current * speedScale * Time.deltaTime);

        if (IsPastDespawnEdge())
        {
            Despawn();
        }
    }

    private bool IsPastDespawnEdge()
    {
        if (mainCamera == null)
        {
            return false;
        }

        float leftEdge = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
        return transform.position.x < leftEdge - despawnBuffer;
    }

    protected virtual void Despawn()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            OnPlayerHit(player);
        }
    }

    public abstract void OnPlayerHit(PlayerController player);
}
