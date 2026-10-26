using System.Collections;
using UnityEngine;

[RequireComponent(typeof(JuicyHealthBar))]
public class BossBarWaiter : MonoBehaviour
{
    JuicyHealthBar bar;
    CanvasGroup group;

    void Awake()
    {
        bar = GetComponent<JuicyHealthBar>();
        group = GetComponent<CanvasGroup>();
        if (!group) group = gameObject.AddComponent<CanvasGroup>();

        bar.boss = null;
        bar.enabled = false;   // stops its Start from running (and disabling itself) too early
        group.alpha = 0f;      // hide the empty bar
    }

    // OnEnable (not Start) so the wait restarts if the bar is deactivated and re-activated
    // by bossfightstart (hidden at scene start, shown when the fight begins).
    void OnEnable()
    {
        StartCoroutine(WaitForBoss());
    }

    IEnumerator WaitForBoss()
    {
        BossController boss = null;
        while (!boss)
        {
            boss = FindFirstObjectByType<BossController>();
            if (!boss) yield return new WaitForSeconds(0.25f);
        }

        bar.boss = boss;
        bar.enabled = true;    // its Start now runs with a valid boss
        group.alpha = 1f;

        // hide the bar once the boss is gone OR dead (the dead boss object is not destroyed)
        while (boss && !boss.IsDead) yield return null;
        group.alpha = 0f;
    }
}