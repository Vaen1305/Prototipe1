using System.Collections.Generic;
using UnityEngine;

public class SpawnDrone : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public int maxObjects = 10;
    public float spawnRate = 2f;
    public bool autoSpawn = true;
    private float nextSpawnTime;
    private List<GameObject> spawnedObjects = new List<GameObject>();


    void Start()
    {
        nextSpawnTime = Time.time + spawnRate;
    }

    void Update()
    {
        spawnedObjects.RemoveAll(item => item == null);

        if(autoSpawn && Time.time >= nextSpawnTime && spawnedObjects.Count < maxObjects)
        {
            SpawnObject();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
    
    public void SpawnObject()
    {
        if (prefabToSpawn == null || spawnedObjects.Count >= maxObjects)
        {
            return;
        }

        Transform spawnTransform = spawnPoint != null ? spawnPoint : transform;
        GameObject newObject = Instantiate(prefabToSpawn, spawnTransform.position, spawnTransform.rotation);  
        spawnedObjects.Add(newObject);
    }
    
    public void ResetSpawnCount()
    {
        spawnedObjects.Clear();
    }
}