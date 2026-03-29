using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; 


public class AI : MonoBehaviour
{

    enum AIState { Patrol, Chase, Search }
    AIState currentState = AIState.Patrol;
    NavMeshAgent navMesh;
    Transform Target;
   [SerializeField] float lookDistance;
    public GameObject cubePrefab;
    public float viewAngle = 90f;
    private Vector3 startPosition;
    private Vector3 lastSeenPosition;
    private bool isSearching = false;
    private bool isAttacking = false;
    private Animator anim;
    public AudioClip StepAudio;
    public AudioSource audioSRC; 
   
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        audioSRC = GetComponent<AudioSource>();
        Target = GameObject.FindWithTag("Player").transform;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) Target = playerObj.transform;
        startPosition = transform.position;
        lastSeenPosition = transform.position;
        anim = GetComponent<Animator>();
    }
    bool CanSeePlayer()
    {
        if (Target == null) return false;
        float distance = Vector3.Distance(transform.position, Target.position);
        if(distance <= lookDistance)
        {
            Vector3 directionToPlayer = (Target.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);
            if(angleBetween <= viewAngle / 2F)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit , lookDistance)) 
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void Update()
    {
        if (Target == null) return;
        if(navMesh.velocity.magnitude > 0.01f)
        {
            if(!audioSRC.isPlaying && StepAudio != null)
            {
                audioSRC.clip= StepAudio;
                audioSRC.loop = true;
                audioSRC.Play();
            }
        }
        else
        {
            if(audioSRC.isPlaying && audioSRC.clip == StepAudio)
            {
                audioSRC.Stop();
            }
        }
        if (CanSeePlayer())
        {
            currentState = AIState.Chase;
            isSearching = false;
            StopAllCoroutines();
            lastSeenPosition = Target.position;
            navMesh.SetDestination(Target.position);
            navMesh.isStopped = false;
            navMesh.SetDestination(Target.position);
            anim.SetFloat("speed", 1f);
            transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));

        }
        else
        {
            if (navMesh.velocity.magnitude > 0.1f) anim.SetFloat("speed", 0.5f);
            else anim.SetFloat("speed", 0f);
            if (currentState == AIState.Chase)
            {
                if (Vector3.Distance(transform.position, lastSeenPosition) > 1.2f)
                {
                    navMesh.SetDestination(lastSeenPosition);
                }
                else if (!isSearching)
                {
                    currentState = AIState.Search;
                    StartCoroutine(SearchAndReturn());
                }
            }
            else if (currentState == AIState.Patrol)
            {
                navMesh.SetDestination(startPosition);
            }
        }
      
       
    }
    IEnumerator SearchAndReturn()
    {
        isSearching = true;
        navMesh.SetDestination(transform.position);
        float timer = 0f;
        while(timer < 3f)
        {
            transform.Rotate(0, 150f * Time.deltaTime, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        currentState = AIState.Patrol;
        navMesh.SetDestination(startPosition);
        while (Vector3.Distance(transform.position, startPosition) < 1.5f)
        {
            yield return null;
        }
        isSearching = false;
    }
    

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * lookDistance);
        Gizmos.DrawRay(transform.position, rightBoundary * lookDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            StartCoroutine(AttackCoolDown());
        }
    }
    IEnumerator AttackCoolDown()
    {
        isAttacking = true;
        anim.SetTrigger("isAttack");
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

}
