using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IDamageable
{
    void TakeDamage(int amount);
}

public class PlayerControlsFight : MonoBehaviour
{
    public float moveSpeed = 6f;
    [Range(0f, 1f)]
    public float attackMoveMultiplier = 0.3f;

    [Header("Dash")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    public float dashInvulnTime = 0.3f;

    [Header("Attack")]
    public int damage = 1;
    public float hitboxDistance = 0.9f;
    public float hitboxRadius = 0.7f;
    public float windupTime = 0.08f;
    public float activeTime = 0.10f;
    public float recoveryTime = 0.20f;
    public LayerMask hittableLayers;

    [Header("Attack Feel")]
    public float jerkSpeed = 4f;
    public float hitStopDuration = 0.05f;
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.1f;

    Rigidbody2D rb;
    Animator anim;

    Vector2 moveInput;
    Vector2 lastMoveDir = Vector2.down;
    Vector2 dashDir;
    Vector2 jerkVelocity;

    bool isDashing;
    bool isAttacking;
    bool canDash = true;

    Coroutine currentAction;
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
        moveInput = ctx.ReadValue<Vector2>();
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

    void StartAction(IEnumerator routine)
    {
        if (currentAction != null) StopCoroutine(currentAction);
        isDashing = false;
        isAttacking = false;
        jerkVelocity = Vector2.zero;
        rb.excludeLayers = 0;
        currentAction = StartCoroutine(routine);
    }

    void FixedUpdate()
    {
        if (isDashing)
            rb.linearVelocity = dashDir * dashSpeed;
        else if (isAttacking)
            rb.linearVelocity = moveInput * moveSpeed * attackMoveMultiplier + jerkVelocity;
        else
            rb.linearVelocity = moveInput * moveSpeed;
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        StartCoroutine(DashCooldown());

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

    public Transform attackSprite;
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        Vector2 aim = GetAimDirection();
        float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        attackSprite.localRotation = Quaternion.Euler(0f, 0f, angle);
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(windupTime);

        hitThisSwing.Clear();

        jerkVelocity = aim * jerkSpeed;

        float timer = 0f;
        while (timer < activeTime)
        {
            DoHitCheck(aim);
            timer += Time.deltaTime;
            yield return null;
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
            if (!hitThisSwing.Add(col)) continue;

            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                //Debug.Log(col.name);
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