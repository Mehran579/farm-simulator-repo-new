using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level4Manager : MonoBehaviour
{
    public HopAway[] allTheKnights;
    public PlayerManager player;
    void OnEnable()
    {
        player.currentState = PlayerManager.PlayerState.Cutscene;
        StartCoroutine(level4routine());
        //Invoke(nameof(TriggerJump), 0.56f);
    }
    void TriggerJump()
    {
        PlayerManager.canMove = false;
        foreach (HopAway k in allTheKnights)
        {
            StartCoroutine(JumpTimeRandomizing(k));
            //k.StartHopping();
        }
    }
    IEnumerator JumpTimeRandomizing(HopAway k)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        k.StartHopping();
    }
    IEnumerator level4routine()
    {
        yield return new WaitForSeconds(0.57f);
        TriggerJump();
        yield return new WaitForSeconds(1.2f);
        CameraMovvingCloser.MoveCameraTowardPlayer();
        yield return new WaitForSeconds(2f);
        DialogueManager.Instance.StartDialogue(player1stline, () =>
        {
            Camera.main.GetComponent<Camera>().orthographicSize += 2.2f;
            foreach (GameObject g in todisable)
                g.SetActive(false);

            SpawnEnemiesAround(amount, radius);
            SpawnEnemiesAround(amount + 3, radius + 1.2f);
            SpawnEnemiesAround(amount + 5, radius + 2.2f);
            SpawnEnemiesAround(amount + 7, radius + 3.2f);

            Invoke(nameof(lastdiaglouge), 0.2f);
        });
    }
    public DialogueData player1stline;

    public GameObject EnemyKnightprefab;
    public int amount = 8;
    public float radius = 3f;
    public void SpawnEnemiesAround(float amount, float raidus)
    {
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < amount; i++)
        {
            float angle = i * (360f / amount) + offset;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * raidus;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * raidus;

            Vector3 spawnPosition = player.gameObject.transform.position + new Vector3(x, y, 0f);

            Instantiate(EnemyKnightprefab, spawnPosition, Quaternion.identity);
        }
    }
    public DialogueData lastdialogue;
    void lastdiaglouge()
    {
        DialogueManager.Instance.StartDialogue(lastdialogue, () =>
        {
            SceneManager.LoadScene(1);
        });
    }
    public GameObject[] todisable;
}
