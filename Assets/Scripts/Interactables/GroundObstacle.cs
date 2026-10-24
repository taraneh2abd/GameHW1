using UnityEngine;

/// Picks a random cactus look each time it's spawned, for visual variety.
/// Sprites share a taller padded canvas (bottom-aligned), so the real visible
/// height is supplied separately rather than trusted from sprite bounds.
public class GroundObstacle : DamagingObstacle
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Sprite[] variants;
    [SerializeField] private float[] variantHeights;

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

        int index = Random.Range(0, variants.Length);
        var sprite = variants[index];
        float height = variantHeights[index];

        spriteRenderer.sprite = sprite;
        boxCollider.size = new Vector2(sprite.bounds.size.x, height);
        boxCollider.offset = new Vector2(0f, height * 0.5f);
    }
}
