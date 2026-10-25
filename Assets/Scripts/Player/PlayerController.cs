using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float groundY;
    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float dashSpeedMultiplier = 6f;
    [SerializeField] private float dashLungeDistance = 2.5f;
    [SerializeField] private float dashMoveSpeed = 12f;
    [SerializeField] private Vector2 duckColliderSize = new Vector2(1f, 0.3f);
    [SerializeField] private Vector2 duckColliderOffset = new Vector2(0f, 0.15f);

    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D hitbox;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction duckAction;

    private Vector2 standColliderSize;
    private Vector2 standColliderOffset;

    private bool grounded = true;
    private bool duckHeld;
    private bool dashHeld;
    private float restX;
    private bool dead;

    public bool IsGrounded => grounded;
    public bool IsDashing => dashHeld;
    public bool IsDead => dead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        standColliderSize = hitbox.size;
        standColliderOffset = hitbox.offset;
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
        dashAction.performed += OnDashPerformed;
        dashAction.canceled += OnDashCanceled;
        duckAction.performed += OnDuckPerformed;
        duckAction.canceled += OnDuckCanceled;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDashPerformed;
        dashAction.canceled -= OnDashCanceled;
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

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashHeld = true;
    }

    private void OnDashCanceled(InputAction.CallbackContext context)
    {
        dashHeld = false;
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
        GameSpeed.DashMultiplier = dashHeld ? dashSpeedMultiplier : 1f;

        float targetX = restX + (dashHeld ? dashLungeDistance : 0f);
        float newX = Mathf.MoveTowards(body.position.x, targetX, dashMoveSpeed * Time.deltaTime);
        body.position = new Vector2(newX, body.position.y);
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
        hitbox.size = ducking ? duckColliderSize : standColliderSize;
        hitbox.offset = ducking ? duckColliderOffset : standColliderOffset;
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
