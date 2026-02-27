using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Animator anim; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }
}