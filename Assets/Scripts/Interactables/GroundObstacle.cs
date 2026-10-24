using UnityEngine;

/// Picks a random cactus look each time it's spawned, for visual variety.
public class GroundObstacle : DamagingObstacle
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Sprite[] variants;

    protected override void Awake()
    {
        base.Awake();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        if (variants == null || variants.Length == 0)
        {
            return;
        }

        var sprite = variants[Random.Range(0, variants.Length)];
        spriteRenderer.sprite = sprite;
        boxCollider.size = sprite.bounds.size;

        float halfHeight = sprite.bounds.extents.y;
        transform.position = new Vector3(transform.position.x, halfHeight, transform.position.z);
    }
}
