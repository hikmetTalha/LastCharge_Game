using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI; //NavMesh Componentlarýna eriþmemizi saðlar.


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
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        Target = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;
        lastSeenPosition = transform.position;
    }
    bool CanSeePlayer()
    {
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
        if (CanSeePlayer())
        {
            currentState = AIState.Chase;
            isSearching = false;
            StopAllCoroutines();
            lastSeenPosition = Target.position;
            navMesh.SetDestination(Target.position);
            transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
        }
        else
        {
         if(currentState == AIState.Chase)
            {
                if (Vector3.Distance(transform.position, lastSeenPosition) > 1.2f)
                {
                    navMesh.SetDestination(lastSeenPosition);
                }
                else
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
    /* void FixedUpdate()
     {
         RaycastHit hit;
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if(Physics.Raycast(ray,out hit))
         {
             if (Input.GetMouseButtonDown(0))
             {
                 navMesh.SetDestination(hit.point);
           GameObject createdCube = Instantiate(cubePrefab, hit.point, transform.rotation);
             Destroy(createdCube, 3f);
             }
         }
     }*/

    private void OnDrawGizmos()
    {
        //MESAFE DAÝRESÝ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
        //GÖRÜÞ HUNÝSÝ
        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * lookDistance);
        Gizmos.DrawRay(transform.position, rightBoundary * lookDistance);
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player")) //Çarptýðý objenin tagi Playersa þunu þunu yap.
        {
            col.gameObject.GetComponent<CharacterMovement>().Charge -= 30.0f;

        }
    }

}
