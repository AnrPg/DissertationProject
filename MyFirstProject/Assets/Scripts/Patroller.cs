using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour
{
    NavMeshAgent agent;
    bool patrolling;
    [SerializeField] Transform[] patrolTargets;
    private int destinationPointIndex;
    private bool arrived;
     
    // Start is called before the first frame update
    void Start()
    {
        agent.GetComponent<NavMeshAgent>();

        // Generate four patrol points at the nodes of a random square area around this Patroller

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
    
}
