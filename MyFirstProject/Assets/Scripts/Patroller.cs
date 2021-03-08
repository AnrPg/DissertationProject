using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour
{
    private const int numOfPatrolTargets = 4; // Should this is changed, then the way patrolTargets[] is populated has also to be changed in Start()

    NavMeshAgent agent;
    bool patrolling;
    [SerializeField] private Transform[] patrolTargets;    
    private int destinationPointIndex;
    private bool arrived;

    [SerializeField] private const float neighborRadius = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolTargets = new Transform[numOfPatrolTargets];

        // Generate four patrol points at the nodes of a random square area around this Patroller
        Vector2 randDiskPoint = neighborRadius * Random.insideUnitCircle + new Vector2(transform.position.x, transform.position.z);
        patrolTargets[0] = SpawnAboveTerrain(null, randDiskPoint.x, randDiskPoint.y).transform;
        patrolTargets[1] = SpawnAboveTerrain(null, randDiskPoint.x + neighborRadius, randDiskPoint.y).transform;
        patrolTargets[2] = SpawnAboveTerrain(null, randDiskPoint.x, randDiskPoint.y + neighborRadius).transform; ;
        patrolTargets[3] = SpawnAboveTerrain(null, randDiskPoint.x + neighborRadius, randDiskPoint.y + neighborRadius).transform;
        //Debug.Log(patrolTargets[0].position + "\n" + patrolTargets[1].position + "\n" + patrolTargets[2].position + "\n" + patrolTargets[3].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.pathPending)
        {
            return;
        }

        if (patrolling)
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                if (!arrived)
                {
                    arrived = true;
                    StartCoroutine("GoToNextPoint");
                }
            }else
            {
                arrived = false;
            }
        }
        else
	    {
            StartCoroutine("GoToNextPoint");
	    }
    }

    IEnumerator GoToNextPoint()
    {
        if (patrolTargets.Length == 0)
        {
            yield break;
        }

        // Preparing to move to next point (after waiting for 2 seconds)
        patrolling = true;
        yield return new WaitForSeconds(2f);
        arrived = false;
        agent.destination = patrolTargets[destinationPointIndex].position;
        destinationPointIndex = (destinationPointIndex + 1) % patrolTargets.Length;
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

    private GameObject SpawnAboveTerrain(Object prefab, float xCord, float zCord, string emptyObjectName= "PatrolTarget")
    {
        // make Vector3 with global coordinates xVal and zVal (Y doesn't matter):
        Vector3 signPosition = new Vector3(xCord, 0, zCord);
        // Retrieve the terrain that is under the point called "signPosition"
        Terrain activeTerrain = GetClosestCurrentTerrain(signPosition);
        // set the Y coordinate according to terrain Y at that point:
        signPosition.y = activeTerrain.SampleHeight(signPosition) + activeTerrain.GetPosition().y;
        // you probably want to create the object a little above the terrain:
        signPosition.y += 0.5f; // move position 0.5 above the terrain
        if (prefab != null)
        {
            return Instantiate(prefab, signPosition, Quaternion.identity) as GameObject;
        }
        else
        {
            GameObject emptyObject = new GameObject(emptyObjectName);
            emptyObject.transform.position = signPosition;
            emptyObject.transform.rotation = Quaternion.identity;
            return emptyObject;
        }
    }
}
