using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UGG.Combat;
using System;

public class EnemyScript : MonoBehaviour
{
    //Declarations
    public event Action<int, int> UpdateHealthBarOnAttacked;
    
    private Animator animator;
    private PlayerCombatSystem playerCombat;
    private EnemyManager enemyManager;
    //private EnemyDetection enemyDetection;
    private CharacterController characterController;

    [Header("Stats")]
    public int CurrentHealth = 3;
    public int MaxHealth = 3;
    public int AttackDamage = 2;
    private float moveSpeed = 1;
    private Vector3 moveDirection;

    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    [SerializeField] private TargetScript TargetObject;

    [SerializeField] private Transform detectionCenter;
    [SerializeField] private float AttackDetectionRang;

    private int AttacktargetCount;
    //»º´æ
    [SerializeField] private Collider[] AttackDetectionTarget = new Collider[1];
    [SerializeField] protected LayerMask enemyLayer;

    /*    [Header("Polish")]
        [SerializeField] private ParticleSystem counterParticle;*/

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        playerCombat = FindObjectOfType<PlayerCombatSystem>();
        //enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();


        MovementCoroutine = StartCoroutine(EnemyMovement());

    }

    IEnumerator EnemyMovement()
    {
        //Waits until the enemy is not assigned to no action like attacking or retreating
        yield return new WaitUntil(() => isWaiting == true);

        int randomChance = UnityEngine.Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = UnityEngine.Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    void Update()
    {
        //Constantly look at player
        transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

        //Only moves if the direction is set
        MoveEnemy(moveDirection);

        AttacktargetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, AttackDetectionRang, AttackDetectionTarget, enemyLayer);

        if (AttacktargetCount == 0)
        {
            for (int i = 0; i < AttackDetectionTarget.Length; i++)
            {
                AttackDetectionTarget[i] = null;
            }

        }
    }

    //Listened event from Player Animation
    public void OnPlayerHit(EnemyScript target,int Damage)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            //enemyDetection.SetCurrentTarget(null);
            isLockedTarget = false;
            OnDamage.Invoke(this);
            

            CurrentHealth -= Damage;

            UpdateHealthBarOnAttacked?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
            {
                Death();
                return;
            }

            animator.SetTrigger("Hit");
            characterController.SimpleMove(transform.position - (transform.forward / 2));
            //transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);



            StopMoving();
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(.5f);
            isStunned = false;
        }
    }

    void Death()
    {
        StopEnemyCoroutines();

        //TargetObject.OnBecameInvisible();
        if (TargetObject != null)
        {
            Destroy(TargetObject.gameObject);
        }

        //this.isRetreating = false;

        this.enabled = false;
        characterController.enabled = false;
        animator.SetTrigger("Death");
        enemyManager.SetEnemyAvailiability(this, false);
    }

    public void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 4);
            isRetreating = false;
            StopMoving();

            //Free 
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    public void SetAttack()
    {
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }


    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            //counterParticle.Play();
        }
        else
        {
            StopMoving();
            //counterParticle.Clear();
            //counterParticle.Stop();
        }
    }

    void MoveEnemy(Vector3 direction)
    {
        //Set movespeed based on direction
        moveSpeed = 1;

        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        //Set Animator values
        animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        //Don't do anything if isMoving is false
        if (!isMoving)
            return;

        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
        Vector3 movedir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        movedir += finalDirection * moveSpeed;// * Time.deltaTime

        characterController.SimpleMove(movedir);

        if (!isPreparingAttack)
            return;

        if(Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
        {
            StopMoving();
            Attack();
            /*            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                            Attack();
                        else*/
            PrepareAttack(false);
        }
            
    }

    private void Attack()
    {
        characterController.SimpleMove(transform.position + (transform.forward / 1));
        animator.SetTrigger("AirPunch");
    }

    public void HitEvent()
    {
        //if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
        if (animator.CheckAnimationTag("Attack"))
        {
            if (AttacktargetCount != 0)
            {

                for (int i = 0; i < AttackDetectionTarget.Length; i++)
                {
                    if (AttackDetectionTarget[i] != null)
                    {
                        AttackDetectionTarget[i].gameObject.GetComponentInChildren<PlayerCombatSystem>().OnBeHitted(AttackDamage);
                        Debug.Log("PlayerBeHitted");
                    }

                }

            }

        }

        PrepareAttack(false);
    }

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        if(characterController.enabled)
            characterController.Move(moveDirection);
    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
                StopCoroutine(RetreatCoroutine);
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if(DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    #region Public Booleans

    public bool IsAttackable()
    {
        return CurrentHealth > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion
}
