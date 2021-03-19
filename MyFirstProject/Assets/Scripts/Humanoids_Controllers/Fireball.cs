using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange;

    private Vector3 originPoint;
    // The following fields should be initialized exactly when the firebal is constructed
    private Vector3 targetPoint;
    private Vector3 movingDirection;    
    private GameObject shooter; 

    void Start()
    {
        movingDirection = targetPoint - transform.position;
        originPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {        
        if (Vector3.Distance(originPoint, transform.position) > attackRange)
        {
            Destroy(this.gameObject);
        }

        moveAutonomously();
        //moveToVanishDirection(targetPoint - transform.position);
    }

    private void moveAutonomously()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    /*
    // Just move to the specified direction (so that when you are at the "targetPoint" the fireball will vanish)
    private void moveToVanishDirection(Vector3 movingDirection)
    {
        transform.Translate(movingDirection * Time.deltaTime);
    }
    */
    public void setShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    public void setTargetPoint(Vector3 targetPoint)
    {
        this.targetPoint = targetPoint;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    void OnTriggerEnter(Collider other)
    {
        ReactiveTarget target = other.GetComponent<ReactiveTarget>();

        // The player who shot this fireball must not collide with it
        if (ObjectInSameHierarchy(shooter.gameObject, other.gameObject))
        {
            return;
        }

        if (target != null)
        {
            target.ReactToHit(movingDirection, damage);
        }
         
        // If you want to be able to hurt both targets and other playerCharacters (if there exists any) uncomment the following block and delete the previous
        
        /*
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();

        if (player != null)
        {
            target.ReactToHit(movingDirection, damage);
        }
        else
        {
            ReactiveTarget target = other.GetComponent<ReactiveTarget>();

            if (target != null)
            {
                target.ReactToHit(movingDirection, damage);
            }
        }
        */

        Destroy(this.gameObject);
    }

    private bool ObjectInSameHierarchy(GameObject gameObject1, GameObject gameObject2)
    {
        return gameObject1.transform.root.Equals(gameObject2.transform.root);
    }

}
