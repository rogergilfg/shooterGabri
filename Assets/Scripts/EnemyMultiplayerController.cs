using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMultiplayerController : MonoBehaviour
{
    [Header("Referencia al Player")]
    [SerializeField] private Transform player;

    [Header("Detección")]
    [SerializeField] private float detectionRange = 100f; 

    [Header("Daño")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool playerDetected = false;
    private bool playerInRange = false;
    private float attackTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

                    playerDetected = true;

        // Buscar al player automáticamente si no está asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Evitar que se incline hacia arriba o abajo
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Actualizar parámetro del Animator
        animator.SetBool("PlayerDetected", playerDetected);

        // Moverse hacia el player si detectado
        if (playerDetected)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }

        // Cooldown del ataque
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        // Atacar si está en rango
        if (playerInRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        attackTimer = attackCooldown;

        // Llamar a TakeDamage del Player
        MultiplayerController playerHealth = player.GetComponent<MultiplayerController>();
        if (playerHealth != null)
        {
            //playerHealth.TakeDamage(damage);
        }
    }

    // Trigger cuando el player entra en rango de ataque
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRange"))
        {
            playerInRange = true;
            animator.SetBool("PlayerInRange", true);
            agent.isStopped = true; // Detener movimiento
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerRange"))
        {
            playerInRange = false;
            animator.SetBool("PlayerInRange", false);
            agent.isStopped = false; // Reanudar movimiento
        }
    }

    // Visualizar el rango de detección en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void TakeDamage(float damage, Player owner)
    {

    }

    public void EnemyDead()
    {

    }
}
