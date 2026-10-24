using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float groundY;
    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float dashSpeedMultiplier = 3f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 0.75f;
    [SerializeField] private GameObject dustPuffPrefab;

    private Rigidbody2D body;
    private Animator animator;
    private InputAction jumpAction;
    private InputAction dashAction;

    private bool grounded = true;
    private float dashTimeLeft;
    private float dashCooldownLeft;
    private bool dead;

    public bool IsGrounded => grounded;
    public bool IsDashing => dashTimeLeft > 0f;
    public bool IsDead => dead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        var map = inputActions.FindActionMap("Gameplay");
        jumpAction = map.FindAction("Jump");
        dashAction = map.FindAction("Dash");
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        dashAction.Enable();
        jumpAction.performed += OnJump;
        dashAction.performed += OnDash;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDash;
        jumpAction.Disable();
        dashAction.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (dead || !grounded)
        {
            return;
        }
        body.linearVelocity = new Vector2(0f, jumpSpeed);
        SpawnDustPuff();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (dead || dashCooldownLeft > 0f)
        {
            return;
        }
        dashTimeLeft = dashDuration;
        dashCooldownLeft = dashCooldown;
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        TickDash();
        TickGroundCheck();

        animator.SetBool("Grounded", grounded);
        animator.speed = GameSpeed.Current / GameSpeed.DefaultBaseSpeed;
    }

    private void TickDash()
    {
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            GameSpeed.DashMultiplier = dashSpeedMultiplier;

            if (dashTimeLeft <= 0f)
            {
                GameSpeed.DashMultiplier = 1f;
            }
        }

        if (dashCooldownLeft > 0f)
        {
            dashCooldownLeft -= Time.deltaTime;
        }
    }

    private void TickGroundCheck()
    {
        bool wasGrounded = grounded;
        grounded = body.position.y <= groundY && body.linearVelocity.y <= 0f;
        if (!grounded)
        {
            return;
        }

        if (!wasGrounded)
        {
            SpawnDustPuff();
        }

        if (body.position.y != groundY)
        {
            body.position = new Vector2(body.position.x, groundY);
        }
        if (body.linearVelocity.y < 0f)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, 0f);
        }
    }

    private void SpawnDustPuff()
    {
        if (dustPuffPrefab != null)
        {
            Instantiate(dustPuffPrefab, new Vector3(body.position.x, groundY, 0f), Quaternion.identity);
        }
    }

    public void PlayHit()
    {
        if (!dead)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void PlayDeath()
    {
        dead = true;
        GameSpeed.DashMultiplier = 1f;
        animator.SetBool("Dead", true);
        animator.SetTrigger("Die");
        body.rotation = -90f;
    }
}
