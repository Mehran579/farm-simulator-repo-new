using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] int maxHealth = 100;
    public int Health => health;

    [Header("Enrage")]
    [SerializeField, Range(0f, 1f)] float enrageAtHealth = 0.75f;   // health fraction that triggers enrage
    [SerializeField] float enrageSpeedMultiplier = 1.8f;            // attacks happen this much faster
    [SerializeField] float enrageProjectileSpeedMultiplier = 1.5f;
    [SerializeField] int enrageExtraMissiles = 3;
    [SerializeField] float enrageShotScaleMultiplier = 1.5f;        // held shot grows bigger
    [SerializeField] int enrageLineDirections = 16;                 // instead of 8

    [Header("Attack Loop")]
    [SerializeField] float startDelay = 1.5f;          // pause after spawning before the first attack
    [SerializeField] float timeBetweenAttacks = 2f;

    [Header("Attack 1: Homing Missiles")]
    [SerializeField] BossProjectile missilePrefab;
    [SerializeField] int missileCount = 5;
    [SerializeField] float missileInterval = 0.4f;     // seconds between missiles appearing on the ring
    [SerializeField] float missileSpeed = 5f;
    [SerializeField] float missileTurnSpeed = 120f;    // degrees per second
    [SerializeField] float missileHomingTime = 3f;     // after this it flies straight
    [SerializeField] float missileLifetime = 6f;
    [SerializeField] float missileSpread = 60f;        // random angle added to the first heading
    [SerializeField] float gatherRadius = 2f;          // ring size the missiles gather on
    [SerializeField] float gatherHoldTime = 0.6f;      // pause once all missiles are in place, before they fire

    [Header("Attack 2: Aimed Shot")]
    [SerializeField] BossProjectile shotPrefab;
    [SerializeField] float holdRadius = 1.5f;          // distance from the boss where the shot hovers
    [SerializeField] float aimTime = 2f;               // shot follows the player during this time
    [SerializeField] float lockTime = 0.5f;            // aim freezes for this long before it fires
    [SerializeField] float shotSpeed = 30f;
    [SerializeField] float shotLifetime = 3f;
    [SerializeField, Range(0.05f, 1f)] float shotStartScale = 0.2f; // fraction of full size when it first appears

    [Header("Attack 3: Line Attack")]
    [SerializeField] GroundHazard hazardPrefab;
    [SerializeField] int hazardsPerLine = 8;
    [SerializeField] float hazardSpacing = 1.2f;
    [SerializeField] float telegraphTime = 1.2f;       // how long each shadow is visible before its damage comes up
    [SerializeField] float stepDelay = 0.1f;           // delay between each hazard appearing along a line
    [SerializeField] float hazardActiveTime = 0.4f;

    [Header("Death")]
    public Sprite deadSprite;
    public GameObject healthBar;
    public DialogueData deathDialogue;
    public SpriteRenderer spriteRenderer;

    int health;
    int lastAttack = -1;
    bool enraged;
    bool fighting;                                     // attack loop already running
    bool dead;                                         // death sequence has begun
    Vector3 baseScale;
    Transform player;
    BossProjectile heldShot;                           // the shot currently hovering before launch
    List<BossProjectile> heldMissiles = new List<BossProjectile>();

    float Speed => enraged ? enrageSpeedMultiplier : 1f;
    float ProjSpeed => enraged ? enrageProjectileSpeedMultiplier : 1f;

    void Awake() { health = maxHealth; baseScale = transform.localScale; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;   // the boss gets spawned, so it can't have scene references
    }

    public void StartFight()
    {
        if (fighting || dead) return;                  // never start twice, never start after death
        fighting = true;
        StartCoroutine(AttackLoop());
    }

    // ------------------------------------------------------------ Attack loop
    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            int next;
            do { next = Random.Range(0, 3); } while (next == lastAttack);   // never the same attack twice in a row
            lastAttack = next;

            switch (next)
            {
                case 0: yield return StartCoroutine(HomingMissiles()); break;
                case 1: yield return StartCoroutine(AimedShot());      break;
                case 2: yield return StartCoroutine(LineAttack());     break;
            }

            yield return new WaitForSeconds(timeBetweenAttacks / Speed);
        }
    }

    // ------------------------------------------------------------ Attack 1
    IEnumerator HomingMissiles()
    {
        int count = missileCount + (enraged ? enrageExtraMissiles : 0);
        heldMissiles.Clear();
        float startAngle = Random.Range(0f, 360f);

        // Phase 1: missiles appear one by one on a ring around the boss (not launched, so no damage yet)
        for (int i = 0; i < count; i++)
        {
            AudioManager.Play(SFX.MissileLaunch);
            BossProjectile m = Instantiate(missilePrefab, transform.position, Quaternion.identity);
            heldMissiles.Add(m);
            PlaceRing(startAngle, count);
            yield return new WaitForSeconds(missileInterval / Speed);
        }

        // Phase 2: brief hold with the full ring
        float t = 0f;
        float hold = gatherHoldTime / Speed;
        while (t < hold)
        {
            PlaceRing(startAngle, count);
            t += Time.deltaTime;
            yield return null;
        }

        // Phase 3: fire at the player, same as before
        foreach (BossProjectile m in heldMissiles)
        {
            if (!m) continue;
            Vector2 toPlayer = (player.position - m.transform.position).normalized;
            float spread = Random.Range(-missileSpread, missileSpread);
            Vector2 dir = Quaternion.Euler(0f, 0f, spread) * toPlayer;
            m.LaunchHoming(dir, missileSpeed * ProjSpeed, player, missileTurnSpeed * ProjSpeed, missileHomingTime, missileLifetime);
        }
        heldMissiles.Clear();
    }

    void PlaceRing(float startAngle, int count)
    {
        for (int i = 0; i < heldMissiles.Count; i++)
        {
            if (!heldMissiles[i]) continue;
            Vector2 outward = Quaternion.Euler(0f, 0f, startAngle + i * (360f / count)) * Vector2.right;
            heldMissiles[i].transform.position = (Vector2)transform.position + outward * gatherRadius;
            heldMissiles[i].transform.right = outward;
        }
    }

    // ------------------------------------------------------------ Attack 2
    IEnumerator AimedShot()
    {
        AudioManager.Play(SFX.ShotCharge);
        heldShot = Instantiate(shotPrefab, transform.position, Quaternion.identity);
        Vector2 dir = Vector2.right;

        float aim = aimTime / Speed;
        float lockT = lockTime / Speed;
        float total = aim + lockT;

        Vector3 fullScale = heldShot.transform.localScale * (enraged ? enrageShotScaleMultiplier : 1f);
        heldShot.transform.localScale = fullScale * shotStartScale;

        float timer = 0f;
        while (timer < total)
        {
            // Track the player, then freeze the aim for the last lockTime so it can be dodged
            if (timer < aim)
                dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

            // Hover at a fixed radius on the line toward the player
            heldShot.transform.position = (Vector2)transform.position + dir * holdRadius;
            heldShot.transform.right = dir;

            // grow while being held
            heldShot.transform.localScale = Vector3.Lerp(fullScale * shotStartScale, fullScale, timer / total);
            if (timer < aim + 0.01f && timer > aim - 0.01f) AudioManager.Play(SFX.ShotLock);

            timer += Time.deltaTime;
            yield return null;
        }

        heldShot.transform.localScale = fullScale;
        AudioManager.Play(SFX.ShotLaunch);
        heldShot.Launch(dir, shotSpeed * ProjSpeed, shotLifetime);
        heldShot = null;
    }

    // ------------------------------------------------------------ Attack 3
    IEnumerator LineAttack()
    {
        int dirs = enraged ? enrageLineDirections : 8;
        float tele = telegraphTime / Speed;
        float step = stepDelay / Speed;

        for (int d = 0; d < dirs; d++)
        {
            Vector2 dir = Quaternion.Euler(0f, 0f, d * (360f / dirs)) * Vector2.right;

            for (int i = 1; i <= hazardsPerLine; i++)
            {
                Vector2 pos = (Vector2)transform.position + dir * (i * hazardSpacing);
                GroundHazard h = Instantiate(hazardPrefab, pos, Quaternion.identity);

                // Shadows appear one by one outward; each one's damage comes up after its telegraph
                h.Init((i - 1) * step, tele, hazardActiveTime);
            }
        }

        yield return new WaitForSeconds(tele + hazardsPerLine * step + hazardActiveTime);
    }

    // ------------------------------------------------------------ Health
    public void TakeDamage(int amount)
    {
        if (dead) return;                              // ignore hits during/after death
        health -= amount;
        if (health > 0) AudioManager.Play(SFX.BossHurt);

        if (!enraged && health > 0 && health <= maxHealth * enrageAtHealth)
        {
            enraged = true;
            StartCoroutine(EnrageSequence());
        }

        if (health <= 0) Die();
    }

    IEnumerator EnrageSequence()
    {
        CameraShake.TriggerShake(0.5f, 0.5f);
        Vector3 originalScale = transform.localScale;

        // Quickly grow
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.3f, t / 0.2f);
            yield return null;
        }

        // Quickly shrink back and tint red
        t = 0f;
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 1.3f, originalScale, t / 0.2f);
            yield return null;
        }
        transform.localScale = originalScale;
    }

    public void Die()
    {
        if (dead) return;                              // guards repeated hits / onclick.cs
        dead = true;

        StopAllCoroutines();                           // stops AttackLoop and any attack in progress
        transform.localScale = baseScale;              // in case enrage animation was cut off

        // remove projectiles that were being held but never launched
        if (heldShot != null) Destroy(heldShot.gameObject);
        foreach (BossProjectile m in heldMissiles)
            if (m) Destroy(m.gameObject);
        heldMissiles.Clear();
        heldShot = null;

        AudioManager.Play(SFX.BossDeath);

        if (spriteRenderer != null && deadSprite != null)
            spriteRenderer.sprite = deadSprite;

        if (healthBar != null)
        {
            // Simulate health bar falling (simple way: disable it or let a separate animation handle it)
            // For now, we'll just hide it to signal the end of the fight
            healthBar.SetActive(false);
        }

        // Start the death sequence
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Initiate death dialogue
        DialogueManager.Instance.StartDialogue(deathDialogue, () =>
        {
            StartCoroutine(FinalCutscene());
        });
        yield return null;
    }

    IEnumerator FinalCutscene()
    {
        // Fade screen and disable activity
        ScreenFade sf = FindFirstObjectByType<ScreenFade>();
        if (sf != null) yield return StartCoroutine(sf.FadeToBlack());

        // First frame: Plane flying sound
        AudioManager.Play(SFX.PlaneFlying);

        // Disable all sounds and logic (pause game or disable components)
        Time.timeScale = 0f;
        AudioManager.Instance?.StopMusic(0.5f);

        // Show 3 frames one by one
        // You'll need to link these images in the Inspector
        for (int i = 0; i < finalFrames.Length; i++)
        {
            GameObject img = finalFrames[i];
            if (img != null)
            {
                img.SetActive(true);

                // Second frame: Explosion sound
                if (i == 1)
                {
                    AudioManager.Play(SFX.Explosion);
                }

                yield return new WaitForSecondsRealtime(0.6f);
                img.SetActive(false);
            }
        }

        // Final result: restart or load next scene
        Time.timeScale = 1f;
        // SceneManager.LoadScene(...) or whatever comes next
    }

    public GameObject[] finalFrames;

    void OnDestroy()
    {
        if (heldShot != null) Destroy(heldShot.gameObject);   // don't leave a frozen shot hovering
        foreach (BossProjectile m in heldMissiles)
            if (m) Destroy(m.gameObject);
    }
}