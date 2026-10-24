using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float groundY;
    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float dashSpeedMultiplier = 4f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float dashCooldown = 0.75f;
    [SerializeField] private float dashLungeDistance = 1.6f;
    [SerializeField] private float duckScale = 0.5f;

    private Rigidbody2D body;
    private Animator animator;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction duckAction;

    private bool grounded = true;
    private bool duckHeld;
    private float restX;
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
        restX = body.position.x;

        var map = inputActions.FindActionMap("Gameplay");
        jumpAction = map.FindAction("Jump");
        dashAction = map.FindAction("Dash");
        duckAction = map.FindAction("Duck");
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        dashAction.Enable();
        duckAction.Enable();
        jumpAction.performed += OnJump;
        dashAction.performed += OnDash;
        duckAction.performed += OnDuckPerformed;
        duckAction.canceled += OnDuckCanceled;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDash;
        duckAction.performed -= OnDuckPerformed;
        duckAction.canceled -= OnDuckCanceled;
        jumpAction.Disable();
        dashAction.Disable();
        duckAction.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (dead || !grounded)
        {
            return;
        }
        body.linearVelocity = new Vector2(0f, jumpSpeed);
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

    private void OnDuckPerformed(InputAction.CallbackContext context)
    {
        duckHeld = true;
    }

    private void OnDuckCanceled(InputAction.CallbackContext context)
    {
        duckHeld = false;
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        TickDash();
        TickGroundCheck();
        TickDuck();

        animator.SetBool("Grounded", grounded);
        animator.speed = GameSpeed.Current / GameSpeed.DefaultBaseSpeed;
    }

    private void TickDash()
    {
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            GameSpeed.DashMultiplier = dashSpeedMultiplier;

            float elapsed = dashDuration - Mathf.Max(dashTimeLeft, 0f);
            float t = Mathf.Clamp01(elapsed / dashDuration);
            float lunge = Mathf.Sin(t * Mathf.PI) * dashLungeDistance;
            body.position = new Vector2(restX + lunge, body.position.y);

            if (dashTimeLeft <= 0f)
            {
                GameSpeed.DashMultiplier = 1f;
                body.position = new Vector2(restX, body.position.y);
            }
        }

        if (dashCooldownLeft > 0f)
        {
            dashCooldownLeft -= Time.deltaTime;
        }
    }

    private void TickGroundCheck()
    {
        grounded = body.position.y <= groundY && body.linearVelocity.y <= 0f;
        if (!grounded)
        {
            return;
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

    private void TickDuck()
    {
        bool ducking = duckHeld && grounded;
        animator.SetBool("Ducking", ducking);
        transform.localScale = new Vector3(1f, ducking ? duckScale : 1f, 1f);
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
