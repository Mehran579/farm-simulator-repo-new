using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public Enemy enemyPrefab;
    public int count = 10;
    public float spawnInterval = 0.5f;   // seconds between each enemy in this group
}

[CreateAssetMenu(menuName = "Waves/Wave")]
public class WAVE : ScriptableObject
{
    public EnemyGroup[] groups;          // spawned one group after another, in this order
}