using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(HitFlash))]
public class playerhealth : MonoBehaviour, IDamageable
{
    public int health = 5;

    [Header("Invulnerability")]
    [SerializeField] float invulnDuration = 1f;     // how long the player can't be hurt after a hit
    [SerializeField] float blinkInterval = 0.1f;    // how fast the sprite blinks

    HitFlash flash;
    float invulnUntil;                              // Time.time value when protection ends

    public bool IsInvulnerable => Time.time < invulnUntil;

    void Awake() => flash = GetComponent<HitFlash>();

    public void TakeDamage(int amount)
    {
        if (IsInvulnerable) return;                 // ignore the hit completely

        health -= amount;
        Debug.Log($"Player health: {health}");

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        invulnUntil = Time.time + invulnDuration;
        flash.Blink(invulnDuration, blinkInterval);

        // Optional tiny freeze for impact:
        HitStop.Do(0.05f);
    }
    public void GrantInvuln(float seconds)
    {
        invulnUntil = Mathf.Max(invulnUntil, Time.time + seconds);
    }
}