using UnityEditor.VersionControl;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed;
    public float attackRange = GameParameters.EnemyAttackRange;
    public float attackCooldown = GameParameters.EnemyAttackCooldown;
    public float playerDetectRange = GameParameters.EnemyDetectRange;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    private int facingDirection = 1; 
    private EnemyState enemyState;
    
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    private string idleState = "IsIdle";
    private string chasingState = "IsChasing";
    private string attackingState = "IsAttacking";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 || player.position.x < transform.position.x && facingDirection == 1){
            Flip();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = moveSpeed * direction;
    }


    void Flip()
    {
            facingDirection *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            //if player in attack range AND cooldown ready
            if(Vector2.Distance(transform.position, player.transform.position) <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            
            else if (Vector2.Distance(transform.position, player.transform.position) > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }
    

    void ChangeState(EnemyState newState)
    {
        //Exit current animation
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool(idleState, false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool(chasingState, false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool(attackingState, false);
        }
        
        // Update state
        enemyState = newState;
        
        // Update new animation
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool(idleState, true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool(chasingState, true);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool(attackingState, true);
        }
    }
}