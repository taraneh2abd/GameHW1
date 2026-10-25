using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public static event Action<int, int> LivesChanged;
    public static event Action<Vector3> Hit;
    public static event Action Died;

    private PlayerController controller;
    private int lives;
    private float invulnerableTimeLeft;

    public int Lives => lives;
    public int MaxLives => maxLives;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        lives = maxLives;
    }

    private void Start()
    {
        LivesChanged?.Invoke(lives, maxLives);
    }

    private void Update()
    {
        if (invulnerableTimeLeft <= 0f)
        {
            return;
        }

        invulnerableTimeLeft -= Time.deltaTime;
        if (invulnerableTimeLeft <= 0f)
        {
            spriteRenderer.enabled = true;
            return;
        }

        bool visible = Mathf.FloorToInt(invulnerableTimeLeft / flashInterval) % 2 == 0;
        spriteRenderer.enabled = visible;
    }

    public void TakeDamage()
    {
        if (invulnerableTimeLeft > 0f || controller.IsDead)
        {
            return;
        }

        lives--;
        LivesChanged?.Invoke(lives, maxLives);

        if (lives <= 0)
        {
            controller.PlayDeath();
            Died?.Invoke();
        }
        else
        {
            controller.PlayHit();
            invulnerableTimeLeft = invulnerabilityDuration;
            Hit?.Invoke(transform.position);
        }
    }
}
