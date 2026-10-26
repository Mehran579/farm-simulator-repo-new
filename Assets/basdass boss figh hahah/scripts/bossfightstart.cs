using UnityEngine;

public class bossfightstart : MonoBehaviour
{
    public GameObject bossPrefab;
    GameObject boss;
    public Transform bossspawn;
    public DialogueData bossd;
    public GameObject bossHealthBar;
    public GameObject[] finalCutsceneFrames;

    bool fightStarted;
    bool introDone;

    void Start()
    {
        // Ensure health bar is hidden immediately on start
        if (bossHealthBar != null) bossHealthBar.SetActive(false);

        // Spawn the boss immediately, but don't start the fight loop yet
        spawnbos_silent();

        // Start the intro conversation; the fight only begins when it finishes
        DialogueManager.Instance.StartDialogue(bossd, () =>
        {
            introDone = true;
            StartFight();
        });
    }

    public void StartFight()
    {
        if (fightStarted) return;   // can only start once
        fightStarted = true;

        // Trigger the roar and music now that the fight is starting
        AudioManager.Play(SFX.BossRoar);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMusic(Music.Boss, 1f);

        if (bossHealthBar != null) bossHealthBar.SetActive(true);

        // Tell the boss he can now attack
        if (boss != null) boss.GetComponent<BossController>().StartFight();
    }

    public void spawnbos_silent()
    {
        if (boss == null)
        {
            boss = Instantiate(bossPrefab, bossspawn.position, Quaternion.identity);

            // Inject references from the scene into the spawned boss prefab
            BossController controller = boss.GetComponent<BossController>();
            if (controller != null)
            {
                controller.healthBar = bossHealthBar;
                controller.finalFrames = finalCutsceneFrames;
            }
        }
    }

    public void spawnbos()
    {
        spawnbos_silent();

        // If there is an intro dialogue, wait for it instead of skipping it
        if (introDone || bossd == null) StartFight();
    }

    public void todoshake()
    {
        //todo;
    }
}