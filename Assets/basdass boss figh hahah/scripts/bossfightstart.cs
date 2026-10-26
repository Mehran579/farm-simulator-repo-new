using UnityEngine;

public class bossfightstart : MonoBehaviour
{
    public GameObject bossPrefab;
    GameObject boss;
    public Transform bossspawn;
    public void spawnbos()
    {
        AudioManager.Play(SFX.BossRoar);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMusic(Music.Boss, 1f);
        Debug.Log("called");
        if(boss==null)
            Instantiate(bossPrefab,bossspawn.position,Quaternion.identity);
    }
    public void todoshake(){
        //todo;
    }
}

