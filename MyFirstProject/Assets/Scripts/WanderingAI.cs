//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    private GameObject _fireball;

    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float obstacleRange = 5.0f;

    private bool _alive;

    void Start()
    {
        _alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_alive)
        {
            WanderAround();
            Vector3[] playersDetectedPlace = DetectPlayer();
            if (playersDetectedPlace != null && playersDetectedPlace[0] != null && playersDetectedPlace[1] != null)
            {
                Hit(playersDetectedPlace);
            }
        }
    }

    private void Hit(Vector3[] playersDetectedPlace)
    {
        if (_fireball == null)
        {
            _fireball = Instantiate(fireballPrefab) as GameObject;            
            _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
            _fireball.transform.rotation = transform.rotation;

            Fireball fireballComponent = _fireball.GetComponent<Fireball>();
            if (fireballComponent != null)
            {
                _fireball.GetComponent<Fireball>().setTargetPoint(playersDetectedPlace[0]);
                _fireball.GetComponent<Fireball>().setShooter(transform.gameObject);
            }            
        }
    }

    public bool ToggleAliveState()
    {
        bool lastState = _alive;
        _alive = !_alive;
        return lastState;
    }

    private void WanderAround()
    {
        bool turnRandomly = Random.Range(0, 100) >= 94;

        if (turnRandomly)
        {
            transform.Rotate(0, Random.Range(-30, 30), 0);
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
        
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        // The sphere's radius has to be the same width as this GameObject's width, to be sure that
        // this GameObject can move forward without being restricted by any obstacle
        float sphereCastRadius = 3.0f;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            sphereCastRadius = meshFilter.mesh.bounds.size.x;
        }
        else
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                sphereCastRadius = renderer.bounds.size.x;
            }
        }

        // If there's an obstacle ahead, make a turn of a random angle (little more than 90 degrees)
        if (Physics.SphereCast(ray, sphereCastRadius, out hit))
        {
            if (hit.distance < obstacleRange)
            {
                float angle = Random.Range(-110, 110);
                transform.Rotate(0, angle, 0);
            }
        }

        // TODO: raycast every some degrees around character to check if player is near, and then turn face-to-face with the player
    }

    // returns the position and rotation this GameObject had when it detected the player. Probably not needed return type...
    private Vector3[] DetectPlayer()
    {
        /*
        for (int incrementingAngle = 0; incrementingAngle <= 360; incrementingAngle += 36)
        {
        
            transform.Rotate(0, incrementingAngle, 0);
        */
            Ray ray = new Ray(transform.TransformPoint(Vector3.forward), transform.position- transform.TransformPoint(Vector3.forward)); // TODO: Vector3.forward ?????
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.GetComponent<PlayerCharacter>())
                {
                    Vector3[] playerFacingPosition = { transform.position, transform.rotation.eulerAngles };
                    return playerFacingPosition;
                }
                /*else if (hit.distance < obstacleRange) // If there's an obstacle ahead, make a turn of a random angle (little more than 90 degrees)
                {
                    float angle = Random.Range(-110, 110);
                    transform.Rotate(0, angle, 0);
                    return null;
                }
                */
            }
        /*
        }
        */
        return null;
    }
}
