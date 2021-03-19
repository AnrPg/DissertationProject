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
    private Vector3 playerPreviousPosition;

    private float terrainWidth;
    private float terrainLength;
    private float xTerrainPos;
    private float zTerrainPos;

    // Start is called before the first frame update
    void Start()
    {
        //Get terrain size
        terrainWidth = terrain.terrainData.size.x;
        terrainLength = terrain.terrainData.size.z;

        //Get terrain position
        xTerrainPos = terrain.transform.position.x;
        zTerrainPos = terrain.transform.position.z;

        // the number of targets is proportional to the terrain size e.g. there's one target every in every two sq. meters
        maxTargets = (int)(targetsSpawnedInNeighbor * terrainWidth * terrainLength / (System.Math.PI * System.Math.Pow(neighborRadius, 2))); // On average, there are "targetsSpawnedInNeighbor" enemies in an area of (maxDistanceInNeighbor ^ 2) sq. meters around player
        //_target = new GameObject[numOfNearTargets];
        //Debug.Log("terrain area: " + terrainWidth * terrainLength + " maxTargets: " + maxTargets);

        playerLastSavedPosition = transform.TransformPoint(GameObject.Find("Character").transform.position); // TODO: put check for non-existence of "Character"
        playerCurrentPosition = transform.TransformPoint(GameObject.Find("Character").transform.position); // TODO: put check for non-existence of "Character"
        playerPreviousPosition = playerCurrentPosition;
        float width = xTerrainPos + terrainWidth;
        float length = zTerrainPos + terrainLength;
        //Debug.Log("xTerrainPos: " + xTerrainPos + " xTerrainPos + terrainWidth: " + width + "\nzTerrainPos: " + zTerrainPos + " zTerrainPos + terrainLength: " + length);
        //Debug.Log("playerCurrentPosition: " + playerCurrentPosition);
        SpawnTarget(targetsSpawnedInNeighbor);
    }

    // Update is called once per frame
    void Update()
    {
        playerPreviousPosition = playerCurrentPosition;
        playerCurrentPosition = GameObject.Find("Character").transform.position; // TODO: put check for non-existence of "Character"
        if (Vector3.Distance(playerLastSavedPosition, playerCurrentPosition) > 2 * neighborRadius)
        {
            
            if (spawnTargets < maxTargets)
            {
                Debug.Log("playerLastSavedPosition: " + playerLastSavedPosition + "\nplayerCurrentPosition: " + playerCurrentPosition + "\nDistance: " + Vector3.Distance(playerLastSavedPosition, playerCurrentPosition));
                Debug.Log("That's why it is spawned...");
                SpawnTarget(targetsSpawnedInNeighbor);
            }
            playerLastSavedPosition = playerCurrentPosition;
        }
        
    }

    public void SpawnTarget(int numToSpawn)
    {
        if (spawnTargets > maxTargets)
        {
            return;
        }

        for (int i = 0; i < numToSpawn; i++)
        {
            spawnTargets++;
               
            Random.InitState(i + System.DateTime.Now.Millisecond);
            float xCoordinate;

            float xLowestPoint = xTerrainPos + terrainWidth > xTerrainPos ? xTerrainPos : xTerrainPos + terrainWidth;
            float xMaximumPoint = xTerrainPos + terrainWidth > xTerrainPos ? xTerrainPos + terrainWidth : xTerrainPos;

            do
            {
                //xCoordinate = (float)(Random.Range(-neighborRadius, neighborRadius) * System.Math.Cos(Random.Range(0, 180)) + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius); // We don't need symmetry around player, because the most targets should be spawned in the front of player
                xCoordinate = (float)(Random.Range(-neighborRadius, neighborRadius) * System.Math.Cos(Random.Range(0, 180)));
                xCoordinate += playerCurrentPosition.x;
            } while ( (xCoordinate < xLowestPoint) || (xCoordinate > xMaximumPoint) );
            
            Random.InitState(i + System.DateTime.Now.Millisecond);
            float zCoordinate;

            float zLowestPoint = zTerrainPos + terrainLength > zTerrainPos ? zTerrainPos : zTerrainPos + terrainLength;
            float zMaximumPoint = zTerrainPos + terrainLength > zTerrainPos ? xTerrainPos + terrainLength : zTerrainPos;

            do
            {
                zCoordinate = (float)(Random.Range(-neighborRadius, neighborRadius) * System.Math.Sin(Random.Range(0, 180)));
                zCoordinate += playerCurrentPosition.z;
            } while ((zCoordinate < zLowestPoint) || (zCoordinate > zMaximumPoint) );
            
            GameObject target = SpawnAboveTerrain(targetPrefab[Random.Range(0, targetPrefab.Length)], xCoordinate, zCoordinate);            

            //Vector3 spawnPositionPlayerIrrelevant = new Vector3(xCoordinate, 0.0f, zCoordinate);
            //Vector2 randDiskPoint = neighborRadius * Random.insideUnitCircle;
            //Vector3 spawnPositionPlayerIrrelevant = new Vector3((float)(randDiskPoint.x + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius), 0.0f, randDiskPoint.y);
            //Debug.Log("spawnPositionPlayerIrrelevant: " + spawnPositionPlayerIrrelevant + "\ndisplacement: " + Mathf.Sign(playerCurrentPosition.x) * 0.55 * neighborRadius);
            //target.transform.position = playerCurrentPosition + spawnPositionPlayerIrrelevant;
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
        return i;
    }

    private Terrain GetClosestCurrentTerrain(Vector3 playerPos)
    {
        //Get all terrain
        Terrain[] terrains = Terrain.activeTerrains;

        //Make sure that terrains length is ok
        if (terrains.Length == 0)
            return null;

        //If just one, return that one terrain
        if (terrains.Length == 1)
            return terrains[0];

        //Get the closest one to the player
        float lowDist = (terrains[0].GetPosition() - playerPos).sqrMagnitude;
        var terrainIndex = 0;

        for (int i = 1; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            Vector3 terrainPos = terrain.GetPosition();

            //Find the distance and check if it is lower than the last one then store it
            var dist = (terrainPos - playerPos).sqrMagnitude;
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return terrains[terrainIndex];
    }

    private GameObject SpawnAboveTerrain(Object prefab, float xCord, float zCord)
    {
        // Compute the direction of player's movement and add a little displacement to that direction
        Vector3 displacement = playerCurrentPosition - playerPreviousPosition;
        displacement.Normalize();
        displacement += new Vector3((float)(0.55 * neighborRadius), 0f, 0f);

        // make Vector3 with global coordinates xVal and zVal (Y doesn't matter):
        Vector3 signPosition = new Vector3(xCord, 0, zCord) + displacement;

        // Retrieve the terrain that is under the point called "signPosition"
        Terrain activeTerrain = GetClosestCurrentTerrain(signPosition);

        // set the Y coordinate according to terrain Y at that point:
        signPosition.y = activeTerrain.SampleHeight(signPosition) + activeTerrain.GetPosition().y;

        // you probably want to create the object a little above the terrain:
        signPosition.y += 0.5f; // move position 0.5 above the terrain
        return Instantiate(prefab, signPosition, Quaternion.identity) as GameObject;
    }
}
