using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{

    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private Animator animator;
    [SerializeField]
    private float speed;
    private Transform player;
    private NavMeshAgent agent;
    [SerializeField]
    private bool following;
    [SerializeField] private Transform[] patrolPoints;
    private int patrolIndex;
    [SerializeField]
    private float health;
    [SerializeField]
    private Weapon weapon;
    private bool reloading;

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
            agent.speed = speed;
            agent.stoppingDistance = 10f;
            animator.SetFloat(Vertical, 1f);
            agent.SetDestination(player.position);
            float distance = (player.position - transform.position).magnitude;
            if (distance <= 10)
            {
                //Disparar
                animator.SetFloat("Vertical", 0);
                transform.LookAt(player.position);
                if(reloading == false)
                {
                    weapon.EnemyShoot(player);

                }            }
        }
        else
        {
            if(patrolPoints.Length > 0)
            {
                animator.SetFloat("Vertical", 0.4f);
                agent.speed = speed * 0.5f;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                float distance = (patrolPoints[patrolIndex].position - transform.position).magnitude;
                if (distance < 10f)
                {
                    patrolIndex += 1;
                    if (patrolIndex >= patrolPoints.Length)
                    {
                        patrolIndex = 0;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject);
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("visto");
            Ray ray = new Ray(transform.position+new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                Debug.DrawRay(transform.position + new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
                if (hit.transform.tag == "Player")
                {
                    following = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.gameObject.tag == "Player")
        {
            Ray ray = new Ray(transform.position + new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(transform.position + new Vector3(0, 1.65f, 0), (player.position - transform.position).normalized);
                if (hit.transform.tag == "Player")
                {
                    following = true;
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;
        following = true;
        if (health<= 0)
        {
            //GameObject ragdollPrefab = Resources.Load<GameObject>("EnemyRagdoll");
            //Instantiate(ragdollPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.GetGameData.CurrentLife -= 10;
            animator.SetTrigger("Hit");
        }
    }

    public void Reload()
    {
        reloading = true;
        animator.SetTrigger("Reload");
        weapon.Reload();
    }

    public void FinishReload()
    {
        reloading = false;
    }
}
