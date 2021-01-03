using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int number;
    public float spawnRadius;
    public bool SpawnOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if (SpawnOnStart) SpawnAll();
    }

    void OnTriggerEnter(Collider other)
    {        
        if (!SpawnOnStart && other.gameObject.tag == "Player")
            SpawnAll();
    }

    private void SpawnAll()
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            else
                i--;
        }
    }
}
