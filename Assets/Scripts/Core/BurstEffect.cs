using UnityEngine;

/// A small one-shot particle burst that configures and plays itself, then
/// destroys its own GameObject when finished. Used for coin pickups, hit
/// impacts, and footstep dust.
[RequireComponent(typeof(ParticleSystem))]
public class BurstEffect : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private int particleCount = 10;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float lifetime = 0.35f;
    [SerializeField] private float size = 0.12f;

    private void Awake()
    {
        var ps = GetComponent<ParticleSystem>();

        var renderer = GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 20;

        var main = ps.main;
        main.startColor = color;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.loop = false;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)particleCount) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        ps.Play();
    }
}
