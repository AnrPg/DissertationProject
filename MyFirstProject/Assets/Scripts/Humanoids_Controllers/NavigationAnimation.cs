using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationAnimation : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    [SerializeField] private List<int> attackAnimationName;
    [SerializeField] private List<int> walkAnimationName;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.Log("No Animator!!!!!!!!!");
        }

        int randWalkAnim = walkAnimationName[Random.Range(0, walkAnimationName.Count)];
        anim.SetFloat("speed", agent.velocity.magnitude);
        //anim.SetInteger("battle", 0);
        //anim.SetBool("armed", true);
        //anim.SetInteger("moving", 1);
    }

    // Update is called once per frame
    void Update()
    {
        /*      int randAttackAnim = attackAnimationName[Random.Range(0, attackAnimationName.Count)];
                Debug.Log(gameObject.name + ": Attack player!");
                anim.SetInteger("battle", 1);
                //anim.SetBool("armed", true);
                anim.SetInteger("moving", randAttackAnim);
        */
    }
}
