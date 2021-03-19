using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    private int hitCounter = 0;
    [SerializeField] private int health;

    /*
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: when I implement the effect to fall to the direction of the hit only the next implementation of this method will be used
    public void ReactToHit(int damage)
    {
        hitCounter++;
        //Debug.Log("Target hit " + hitCounter + " times!");

        health -= damage;
        if (health <= 0)
        {
           StartCoroutine(Die());
           GameObject.Find("Controller").GetComponent<SceneController>().spawnTarget(); // TODO: decide if we need to have constant number of targets in scene
        }
    }
    */
    public void ReactToHit(Vector3 hitDirection, int damage)
    {
        hitCounter++;
        //Debug.Log("Target hit " + hitCounter + " times!");

        if (isAlive())
        {
            health -= damage;

            if (!isAlive())
            {
                Debug.Log(gameObject.name + " is dead");

                // TODO: when I implement the effect to fall to the direction of the hit only the first coroutine will be useful (probably...)
                if (hitDirection != null)
                {
                    StartCoroutine(Die(hitDirection));
                }/*
                else
                {
                    StartCoroutine(Die());
                }*/

                GameObject.Find("Controller").GetComponent<SceneController>().SpawnTarget(1);
            }
        }
    }

    internal bool isAlive()
    {        
        return health > 0;
    }

    /*
    private IEnumerator Die()
    {
        WanderingAI wanderingAI = this.GetComponent<WanderingAI>();
        if (wanderingAI != null)
        {
            wanderingAI.toggleAliveState();
        }
        Destroy(this.gameObject);
    }
    */

    private IEnumerator Die(Vector3 hitDirection)
    {
        /*
        WanderingAI wanderingAI = this.GetComponent<WanderingAI>();
        if (wanderingAI != null)
        {
            wanderingAI.ToggleAliveState();
        }

        // Compute whereto to fall according to the direction of the fatal hit
        float angle = Vector3.Angle(hitDirection, this.transform.position);
        //Debug.Log("Angle: " + angle + " " + hitCounter);

        if (angle < 40)// back
        {
            //Debug.Log("Hit from back" + hitCounter);
            //Debug.Log("Fall to the front" + hitCounter);
            this.transform.Rotate(90, 0, 0);
            yield return new WaitForSeconds(3f);
            //this.transform.Rotate(-90, 0, 0);
        }        
        else if (angle <= 180 && angle >= 120)// front
        {
            //Debug.Log("Hit from front" + hitCounter);
            //Debug.Log("Fall to the back" + hitCounter);
            this.transform.LookAt(hitDirection);
            this.transform.Rotate(-90, 0, 0);
            yield return new WaitForSeconds(3f);
            //this.transform.Rotate(90, 0, 0);
        }
        else //if (angle >= 50 && angle < 90)
        {
            //Debug.Log("Hit from side" + hitCounter);
            Vector3 cross = Vector3.Cross(this.transform.position, hitDirection);
            //Debug.Log("Cross product: " + cross.y + " " + hitCounter);

            if (cross.y >= 0) // Right
            {
                //Debug.Log("Fall to the left" + hitCounter);
                this.transform.Rotate(0, 0, -90);
                yield return new WaitForSeconds(3f);
                //this.transform.Rotate(0, 0, 90);
            }
            else // left
            {
                //Debug.Log("Fall to the right" + hitCounter);
                this.transform.Rotate(0, 0, 90);
                yield return new WaitForSeconds(3f);
                //this.transform.Rotate(0, 0, -90);
            }
        }

        */
        yield return new WaitForEndOfFrame();
        Patroller patroller = transform.GetComponent<Patroller>();
        if (patroller != null)
        {
            foreach (Transform patrolPoint in patroller.PatrolTargets)
            {
                Destroy(patrolPoint.gameObject);
            }
        }
        Destroy(this.gameObject);
    }           
}
