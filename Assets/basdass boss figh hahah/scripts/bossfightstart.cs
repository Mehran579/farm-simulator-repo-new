using UnityEngine;

public class bossfightstart : MonoBehaviour
{
    public GameObject bossPrefab;
    GameObject boss;
    public Transform bossspawn;
    public void spawnbos()
    {
        Debug.Log("called");    
        if(boss==null)
            Instantiate(bossPrefab,bossspawn.position,Quaternion.identity);
    }
    public void todoshake(){
        //todo;
    }
}

