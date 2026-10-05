using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IDamageable
{
    void TakeDamage(int amount);
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControlsFight : MonoBehaviour
{
    public float moveSpeed = 6f;
    [Range(0f, 1f)]
    [SerializeField] float attackMoveMultiplier = 0.3f; // walk speed while attacking

    [Header("Dash")]
    [SerializeField] float dashSpeed = 16f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 0.5f;
    [SerializeField] float dashInvulnTime = 0.3f;   

    [Header("Attack")]
    [SerializeField] int damage = 1;
    [SerializeField] float hitboxDistance = 0.9f;   // how far in front of the player the circle sits
    [SerializeField] float hitboxRadius = 0.7f;
    [SerializeField] float windupTime = 0.08f;      // no damage yet
    [SerializeField] float activeTime = 0.10f;      // hitbox is live
    [SerializeField] float recoveryTime = 0.20f;    // swing is finishing
    [SerializeField] LayerMask hittableLayers;

    [Header("Attack Feel")]
    [SerializeField] float jerkSpeed = 4f;           // lurch speed toward the attack direction during the active phase
    [SerializeField] float hitStopDuration = 0.05f;  // freeze time when a swing connects
    [SerializeField] float shakeDuration = 0.15f;    // camera shake when a swing connects
    [SerializeField] float shakeMagnitude = 0.1f;

    Rigidbody2D rb;
    Animator anim;

    Vector2 moveInput;
    Vector2 lastMoveDir = Vector2.down;
    Vector2 dashDir;
    Vector2 jerkVelocity;

    bool isDashing;
    bool isAttacking;
    bool canDash = true;

    Coroutine currentAction;                         // the ONE dash/attack routine allowed to run
    readonly HashSet<Collider2D> hitThisSwing = new HashSet<Collider2D>();

    playerhealth playerhealt;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerhealt = GetComponent<playerhealth>();
        anim = GetComponent<Animator>();
    }
    void OnDisable() => rb.excludeLayers = 0;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();        // (0,0) automatically when keys are released
        if (moveInput != Vector2.zero) lastMoveDir = moveInput.normalized;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDash)
            StartAction(DashRoutine());
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isDashing && !isAttacking)
            StartAction(AttackRoutine());
    }

    // ------------------------------------------------------------ One action at a time
    void StartAction(IEnumerator routine)
    {
        if (currentAction != null) StopCoroutine(currentAction);
        isDashing = false;
        isAttacking = false;
        jerkVelocity = Vector2.zero;
        rb.excludeLayers = 0;                      // add this line
        currentAction = StartCoroutine(routine);
    }

    // ------------------------------------------------------------ Movement
    void FixedUpdate()
    {
        if (isDashing)
            rb.linearVelocity = dashDir * dashSpeed;
        else if (isAttacking)
            rb.linearVelocity = moveInput * moveSpeed * attackMoveMultiplier + jerkVelocity;
        else
            rb.linearVelocity = moveInput * moveSpeed;
    }

    // ------------------------------------------------------------ Dash
    IEnumerator DashRoutine()
    {
        canDash = false;
        StartCoroutine(DashCooldown());   // separate, NOT cached: an attack must not be able to cancel it

        dashDir = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDir;
        isDashing = true;
        playerhealt.GrantInvuln(dashInvulnTime);
        rb.excludeLayers = hittableLayers;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.excludeLayers = 0;
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ------------------------------------------------------------ Attack
    [SerializeField] Transform attackSprite;
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        Vector2 aim = GetAimDirection();  // locked for the whole swing
        float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        attackSprite.localRotation = Quaternion.Euler(0f, 0f, angle);
        anim.SetTrigger("attack");

        // ANIMATION TRIGGER (main spot): the swing starts here, so fire the attack animation now.
        //   animator.SetTrigger("Attack");
        //   `aim` is the swing direction if you need to rotate the slash effect.

        yield return new WaitForSeconds(windupTime);

        // ANIMATION TRIGGER (alternative spot): put it here instead if you want the slash
        // to appear only when the hitbox goes live.

        hitThisSwing.Clear();             // new swing, nobody has been hit yet

        // Small lurch toward the attack direction for the length of the active phase.
        jerkVelocity = aim * jerkSpeed;

        float timer = 0f;
        while (timer < activeTime)
        {
            DoHitCheck(aim);
            timer += Time.deltaTime;
            yield return null;            // wait one frame
        }

        jerkVelocity = Vector2.zero;

        yield return new WaitForSeconds(recoveryTime);
        isAttacking = false;
    }

    void DoHitCheck(Vector2 aim)
    {
        Vector2 center = rb.position + aim * hitboxDistance;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(center, hitboxRadius, hittableLayers))
        {
            if (!hitThisSwing.Add(col)) continue;   // already hit this swing -> skip

            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                Debug.Log(col.name);
                HitStop.Do(hitStopDuration);
                CameraShake.TriggerShake(shakeDuration, shakeMagnitude);
            }
        }
    }

    Vector2 GetAimDirection()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = mouseWorld - rb.position;
        return dir.sqrMagnitude > 0.01f ? dir.normalized : lastMoveDir;
    }
    
}