using Photon.Realtime;
using System;
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

    [Header("Vida")]
    [SerializeField] private float maxLife = 100f;
    private float life;

    [Header("Daño")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool playerDetected = false;
    private bool playerInRange = false;
    private float attackTimer = 0f;

    // 👇 El SpawnManager se suscribe a este evento para saber cuándo muere el enemigo
    public event Action<GameObject> OnDeath;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        life = maxLife;
        playerDetected = true;

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
        direction.y = 0f;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        animator.SetBool("PlayerDetected", playerDetected);

        if (playerDetected)
            agent.SetDestination(player.position);
        else
            agent.ResetPath();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (playerInRange && attackTimer <= 0f)
            AttackPlayer();
    }

    private void AttackPlayer()
    {
        attackTimer = attackCooldown;

        MultiplayerController playerHealth = player.GetComponent<MultiplayerController>();
        if (playerHealth != null)
        {
            //playerHealth.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRange"))
        {
            playerInRange = true;
            animator.SetBool("PlayerInRange", true);
            agent.isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerRange"))
        {
            playerInRange = false;
            animator.SetBool("PlayerInRange", false);
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void TakeDamage(float dmg, Player owner)
    {
        life -= dmg;
        Debug.Log($"[Enemy] Vida restante: {life}");

        if (life <= 0f)
            EnemyDead();
    }

    public void EnemyDead()
    {
        Debug.Log("[Enemy] Muerto");

        // 👇 Notifica al SpawnManager antes de desactivarse
        OnDeath?.Invoke(gameObject);

        gameObject.SetActive(false);
    }
}