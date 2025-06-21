using UnityEngine;

public enum ZombieState
{
    Idle,
    Walk,
    Attack,
    GetHit,
    Die
}

public class ZombieController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;

    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float health = 100f;
    public float attackCooldown = 2f;

    private ZombieState currentState;
    private Transform target;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        ChangeState(ZombieState.Idle);
    }

    void Update()
    {
        if (isDead) return;

        FindTarget();
        UpdateStateMachine();
    }

    void UpdateStateMachine()
    {
        switch (currentState)
        {
            case ZombieState.Idle:
                HandleIdleState();
                break;
            case ZombieState.Walk:
                HandleWalkState();
                break;
            case ZombieState.Attack:
                HandleAttackState();
                break;
            case ZombieState.GetHit:
                HandleGetHitState();
                break;
            case ZombieState.Die:
                HandleDieState();
                break;
        }
    }

    void HandleIdleState()
    {
        agent.isStopped = true;

        if (target != null && Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            ChangeState(ZombieState.Walk);
        }
    }

    void HandleWalkState()
    {
        if (target == null)
        {
            ChangeState(ZombieState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            ChangeState(ZombieState.Attack);
        }
        else if (distanceToTarget > detectionRange)
        {
            ChangeState(ZombieState.Idle);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    void HandleAttackState()
    {
        agent.isStopped = true;

        if (target == null)
        {
            ChangeState(ZombieState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            ChangeState(ZombieState.Walk);
        }
        else if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void HandleGetHitState()
    {
        agent.isStopped = true;
        // GetHit animation will transition back automatically via animator
    }

    void HandleDieState()
    {
        agent.isStopped = true;
        // Death is final - no transitions out
    }

    void ChangeState(ZombieState newState)
    {
        currentState = newState;
        animator.SetInteger("State", (int)newState);
    }

    void FindTarget()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void PerformAttack()
    {
        // Trigger attack animation
        animator.SetTrigger("AttackTrigger");

        // Deal damage to target if in range
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            // Add your damage logic here
            Debug.Log("Zombie attacks!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            ChangeState(ZombieState.Die);
        }
        else
        {
            ChangeState(ZombieState.GetHit);
            // Automatically return to previous state after hit animation
            Invoke("ReturnFromHit", 0.5f);
        }
    }

    void ReturnFromHit()
    {
        if (currentState == ZombieState.GetHit && !isDead)
        {
            if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                ChangeState(ZombieState.Attack);
            }
            else if (target != null && Vector3.Distance(transform.position, target.position) <= detectionRange)
            {
                ChangeState(ZombieState.Walk);
            }
            else
            {
                ChangeState(ZombieState.Idle);
            }
        }
    }
}