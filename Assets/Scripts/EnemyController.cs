using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    private Animator animator;
    [SerializeField]
    private float speed;
    private Transform player;
    private NavMeshAgent agent;
    private bool following;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (following == true)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Ray ray = new Ray(transform.position+new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(transform.position + new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
                if (hit.transform.tag == "Player")
                {
                    following = true;
                }
            }
        }
    }
}
