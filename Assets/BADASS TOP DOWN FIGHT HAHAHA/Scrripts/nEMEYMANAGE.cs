using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nEMEMYMANAGE : MonoBehaviour
{
    public WAVE[] waves;
    public Transform player;
    public Transform[] targets;
    public float spawnRadius = 10f;
    public float timeBetweenWaves = 3f;

    readonly List<Enemy> aliveEnemies = new List<Enemy>();

    IEnumerator Start()
    {
        foreach (WAVE wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitUntil(AllEnemiesDead);
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        //Debug.Log("all waves cleared");
        winEnd.SetActive(true);
    }
    public GameObject winEnd;
    IEnumerator SpawnWave(WAVE wave)
    {
        foreach (EnemyGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Vector2 direction = Random.insideUnitCircle.normalized;
                float distance = Random.Range(8f, 12f);

                Vector2 pos = (Vector2)player.position + direction * distance;
                Enemy e = Instantiate(group.enemyPrefab, pos, Quaternion.identity);
                e.Init(targets);
                aliveEnemies.Add(e);

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    bool AllEnemiesDead()
    {
        aliveEnemies.RemoveAll(e => e == null);
        return aliveEnemies.Count == 0;
    }
}