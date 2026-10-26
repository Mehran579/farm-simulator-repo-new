using UnityEngine;

public class onclick : MonoBehaviour
{
    public void EndFightAndPlayCutscene()
    {
        BossController boss = FindFirstObjectByType<BossController>();
        if (boss == null)
        {
            Debug.LogWarning("No BossController found in scene to end the fight.");
            return;
        }

        // Die() handles everything (stop attacks, clear projectiles, close intro dialogue,
        // open death dialogue), exactly like a natural death. Extra clicks are ignored.
        if (!boss.IsDead) boss.Die();
    }
}