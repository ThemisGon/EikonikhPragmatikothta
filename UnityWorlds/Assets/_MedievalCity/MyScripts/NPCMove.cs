using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Animator anim; // 1. Προσθέτουμε τον Animator!

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // 2. Βρίσκει τον Animator του NPC
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        // 3. Στέλνει την τρέχουσα ταχύτητα του NPC στον Animator
        // Αν το NPC τρέχει, το magnitude είναι π.χ. 6. Αν σταματήσει, είναι 0!
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }
}