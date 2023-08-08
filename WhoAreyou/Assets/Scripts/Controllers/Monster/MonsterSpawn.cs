using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{    
    [SerializeField]
    public int poolCount = 10;
    [SerializeField]
    public GameObject monster;
    public List<GameObject> monsterObjectPool;
    public BoxCollider area;

    // Start is called before the first frame update
    void Start()
    {
        monsterObjectPool = new List<GameObject>();
        area = GetComponent<BoxCollider>();
        
        StopAllCoroutines();

        for (int i = 0; i < poolCount; i++)
        {
            GameObject gameObject = Instantiate(monster);
            // monster.SetActive(false);              // death 구현 시 수정 예정
            monsterObjectPool.Add(gameObject);

            StartCoroutine(Spawn(i));
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 basePosition = transform.position;
        Vector3 size = area.size;

        float posX = basePosition.x + UnityEngine.Random.Range(-1*(size.x/2f), size.x/2f);
        float posZ = basePosition.z + UnityEngine.Random.Range(-1*(size.z/2f), size.z/2f);

        Vector3 spawnPos = new Vector3(posX, 0f, posZ);

        return spawnPos;
    }

    private IEnumerator Spawn(int index)
    { 
      
        GameObject selectedPrefab = monsterObjectPool[index];

        /*
        if (selectedPrefab.activeSelf == false)
        {
            selectedPrefab.SetActive(true);
        }
        */

        selectedPrefab.transform.position = GetRandomPosition();

        yield return new WaitForSecondsRealtime(1.0f);
    }
}
