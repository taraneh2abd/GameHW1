using UnityEngine;

/// Ground obstacle's airborne counterpart: hovers above standing height so
/// staying grounded clears it, but a mistimed jump flies straight into it.
public class FlyingObstacle : DamagingObstacle
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite frameA;
    [SerializeField] private Sprite frameB;
    [SerializeField] private float flapInterval = 0.15f;

    private float timer;
    private bool showingA = true;

    protected override void Awake()
    {
        base.Awake();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    protected override void Update()
    {
        base.Update();

        timer += Time.deltaTime;
        if (timer >= flapInterval)
        {
            timer = 0f;
            showingA = !showingA;
            spriteRenderer.sprite = showingA ? frameA : frameB;
        }
    }
}
