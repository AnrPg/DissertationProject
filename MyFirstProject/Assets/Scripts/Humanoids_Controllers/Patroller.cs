using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour
{
    private const int numOfPatrolTargets = 4; // Should this is changed, then the way patrolTargets[] is populated has also to be changed in Start()

    NavMeshAgent agent;
    Animator anim;
    bool patrolling;
    [SerializeField] private List<int> attackAnimationName;
    [SerializeField] private List<int> walkAnimationName;
    [SerializeField] private Transform[] patrolTargets;
    private Transform target;
    [SerializeField] private Transform eye;
    private Vector3 lastKnownPlayerPosition;
    private int destinationPointIndex;
    private bool arrived;

    //[SerializeField] private Transform eyeCylinder;

    [SerializeField] private const float neighborRadius = 20.0f;

    public Transform[] PatrolTargets { get => patrolTargets; set => patrolTargets = value; }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.Log("No Animator!!!!!!!!!");
        }

        target = GameObject.FindGameObjectWithTag("Player").transform;

        //eyeCylinder = transform.Find("Cylinder");
        patrolTargets = new Transform[numOfPatrolTargets];

        lastKnownPlayerPosition = target.transform.position;
        //Debug.Log(target.position + "\n" + transform.Find("Eye").name + ": " + transform.Find("Eye").transform.position + "\n" + transform.position + "\n" + (target.transform.position - eye.position).ToString() );

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
        //lastKnownPlayerPosition = target.transform.position;
        //Debug.Log(transform.gameObject.name + ": " + target.position + "\n" + transform.Find("Eye").name + ": " + transform.Find("Eye").transform.position + "\n" + transform.position + "\n" + (target.transform.position - eye.position).ToString());

        if (!agent.isOnNavMesh)
        {
            Destroy(this.gameObject);
        }

        if (agent.pathPending)
        {
            return;
        }
                
        if (patrolling)
        {
            anim.SetInteger("battle", 0);

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                if (!arrived)
                {
                    arrived = true;
                    StartCoroutine("GoToNextPoint");
                }
            }
            else
            {
                arrived = false;
            }
        }

        if (CanSeeTarget())
        {
            agent.SetDestination(target.transform.position);
            
            patrolling = false;
            //Debug.Log(transform.gameObject.name+ ": Aha! Found you! You are gonna die now!" + "\n" + agent.remainingDistance + "\n" + agent.stoppingDistance);

            if (agent.remainingDistance < agent.stoppingDistance + 0.1)
            {
                int randAttackAnim = attackAnimationName[Random.Range(0, attackAnimationName.Count)];
                anim.SetInteger("battle", randAttackAnim);
                Debug.Log(gameObject.name + ": Attack player!");
            }
            else
            {
                // Has no effect at the moment, because at the end of Update() the speed is set again
                anim.SetInteger("battle", 0);
                anim.SetFloat("speed", 6);
                //anim.SetFloat("speed", 6);
                //return;
            }
        }
        else
	    {
            anim.SetInteger("battle", 0);
            
            if (!patrolling)
            {
                //Debug.Log(gameObject.name + ": I lost you, but I am coming for you!");
                agent.SetDestination(lastKnownPlayerPosition);
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    patrolling = true;
                    Debug.Log(gameObject.name + ": Back to our base...");
                    StartCoroutine("GoToNextPoint");
                }
            }
            //anim.SetInteger(randWalkAnim, 1);
            //anim.SetFloat(randWalkAnim, agent.velocity.sqrMagnitude);
        }
        anim.SetFloat("speed", agent.velocity.magnitude); // Calibrate the agents speed and the animator's blending tree speed threshold.
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

    private bool CanSeeTarget()
    {
        bool canSee = false;
        Ray ray = new Ray(eye.position, target.transform.position - eye.position);
        //StartCoroutine( CreateCylinderBetweenPoints(eye.position, target.transform.position - eye.position, 0.2f) );
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != target)
            {
                canSee = false;
            }
            else
            {
                lastKnownPlayerPosition = target.transform.position;
                //Debug.Log(lastKnownPlayerPosition);
                canSee = true;
            }
        }

        return canSee;
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


/*
NavMeshes, home screen and animations

- Experimenting with NavMeshes and NavMeshAgent following the instructions found at the online unnity tutorial found at "https://learn.unity.com/project/unity-navigation-fundamentals"
- Created the home screen with the minimum functionality on it
- Put animations on characters
 */