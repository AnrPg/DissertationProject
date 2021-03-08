using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int health = 10;

    public void Hurt(int damage)
    {
        health -= damage;
        Debug.Log("Player's health: " + health);
        // TODO: have a hit-by-blast effect (translate player some distance in the direction of the hit)
    }

    public void ReactToHit(Vector3 hitDirection, int damage)
    {
        
        health -= damage;
        if (health <= 0)
        {
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

    private IEnumerator Die(Vector3 hitDirection)
    {
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
        else /*if (angle >= 50 && angle < 90)*/
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

        Destroy(this.gameObject);
    }
}
