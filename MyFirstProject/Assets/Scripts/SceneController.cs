using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    //public const int numOfNearTargets = 5;
    [SerializeField] private const float neighborRadius = 50.0f; // When player is so much away from "playerLastSavedPosition" then he enters in a new neighbor and new targets must be spawned
    [SerializeField] private const int targetsSpawnedInNeighbor = 3;

    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject[] targetPrefab;
    [SerializeField] private GameObject monstersParentObject;
    //private GameObject[] _target;
    private int maxTargets = 0;
    private int spawnTargets = 0;
    private Vector3 playerLastSavedPosition;
    private Vector3 playerCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        // the number of targets is proportional to the terrain size e.g. there's one target every in every two sq. meters
        maxTargets = (int)(targetsSpawnedInNeighbor * terrain.terrainData.size.x * terrain.terrainData.size.z / (System.Math.PI * System.Math.Pow(neighborRadius, 2))); // On average, there are "targetsSpawnedInNeighbor" enemies in an area of (maxDistanceInNeighbor ^ 2) sq. meters around player
        //_target = new GameObject[numOfNearTargets];
        //Debug.Log("terrain area: " + terrain.terrainData.size.x * terrain.terrainData.size.z + " maxTargets: " + maxTargets);

        playerLastSavedPosition = GameObject.Find("Character").transform.position;
        SpawnTarget(targetsSpawnedInNeighbor);
    }

    // Update is called once per frame
    void Update()
    {
        playerCurrentPosition = GameObject.Find("Character").transform.position;
        if (Vector3.Distance(playerLastSavedPosition, playerCurrentPosition) > 2 * neighborRadius)
        {
            //Debug.Log("playerLastSavedPosition: " + playerLastSavedPosition + "\nplayerCurrentPosition: " + playerCurrentPosition + "\nDistance: " + Vector3.Distance(playerLastSavedPosition, playerCurrentPosition) );
            if (spawnTargets < maxTargets)
            {
                SpawnTarget(targetsSpawnedInNeighbor);
            }
            playerLastSavedPosition = playerCurrentPosition;
        }
        
    }

    public void SpawnTarget(int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            spawnTargets++;

            Random.InitState(i + System.DateTime.Now.Millisecond);
            GameObject target = Instantiate(targetPrefab[Random.Range(0, targetPrefab.Length)]) as GameObject; // Remove ternary operator if numOfTargetPrefabs more than 2
            float xCoordinate = (float)(Random.Range(-neighborRadius, neighborRadius) * System.Math.Cos(Random.Range(0, 180)) + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius); // We don't need symmetry around player, because the most targets should be spawned in the front of player
            Random.InitState(i + System.DateTime.Now.Millisecond);
            float zCoordinate = (float)(Random.Range(-neighborRadius, neighborRadius) * System.Math.Sin(Random.Range(0, 180)));
            Vector3 spawnPositionPlayerIrrelevant = new Vector3(xCoordinate, 0.0f, zCoordinate);

            //Vector2 randDiskPoint = neighborRadius * Random.insideUnitCircle;
            //Vector3 spawnPositionPlayerIrrelevant = new Vector3((float)(randDiskPoint.x + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius), 0.0f, randDiskPoint.y);
            //Debug.Log("spawnPositionPlayerIrrelevant: " + spawnPositionPlayerIrrelevant + "\ndisplacement: " + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius);
            target.transform.position = playerCurrentPosition + spawnPositionPlayerIrrelevant;
            //target.transform.Rotate(0, Random.Range(0, 360), 0);
            target.transform.parent = monstersParentObject.transform;
        }
    }

    /*
     public void spawnTarget()
    {
        int emptySlot = findFirstempty(_target);
        if ( emptySlot < _target.Length )
        {
            spawnTargets++;
            _target[emptySlot] = Instantiate(targetPrefab[Random.Range(0, numOfTargetPrefabs - 1) < 0.5f ? 0 : 1]) as GameObject; // Remove ternaary operator if numOfTargetPrefabs more than 2
            _target[emptySlot].transform.position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));
            _target[emptySlot].transform.Rotate(0, Random.Range(0, 360), 0);
            _target[emptySlot].transform.parent = monstersParentObject.transform;
        }
       
    }
     */

    private int findFirstempty(Object[] array)
    {
        int i = 0;
        while ( i < array.Length && array[i] != null)
        {
            i++;
        }
        Debug.Log("findFirstempty: " + i);
        return i;
    }
}
