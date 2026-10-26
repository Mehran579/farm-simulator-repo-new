using UnityEngine;

public class onclick : MonoBehaviour
{
    public void EndFightAndPlayCutscene()
    {
        // Find the boss in the scene to trigger his death sequence
        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null)
        {
            boss.Die();
        }
        else
        {
            Debug.LogWarning("No BossController found in scene to end the fight.");
        }
    }
}
