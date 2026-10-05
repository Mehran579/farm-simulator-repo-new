using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nEMEMYMANAGE : MonoBehaviour
{
    [SerializeField] WAVE[] waves;            // drag your wave assets in, in order
    [SerializeField] Transform player;
    [SerializeField] Transform[] targets;         // the 4 track points + the player
    [SerializeField] float spawnRadius = 10f;     // enemies appear on a circle this far from the player
    [SerializeField] float timeBetweenWaves = 3f;

    readonly List<Enemy> aliveEnemies = new List<Enemy>();

    // Start can be a coroutine: Unity runs it as one automatically
    IEnumerator Start()
    {
        foreach (WAVE wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));      // spawn every enemy in this wave
            yield return new WaitUntil(AllEnemiesDead);        // wait until the player kills them all
            yield return new WaitForSeconds(timeBetweenWaves); // short breather
        }

        Debug.Log("All waves cleared");
    }

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
        aliveEnemies.RemoveAll(e => e == null);   // a destroyed enemy shows up as null
        return aliveEnemies.Count == 0;
    }
}