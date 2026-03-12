using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    public float Charge = 100;
    [SerializeField] int battery = 0;
    [SerializeField] bool hasKey = false;
    bool unlockGate = false;

    [Header("Character Traits")]

    [Tooltip("Karakterin hýzýdýr.")] public float speed;

    

    [Header("Move Direction")]

    [SerializeField] private Vector3 moveDirection;

    private Rigidbody rb;

    [SerializeField] Camera cam;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
       // StartCoroutine(Rutin());
    }

    void Update()
    {
        #region MOVEMENT_PART
        Charge -= 2f*Time.deltaTime;
        speed = Mathf.Clamp(speed , 10f, 15f);
        Movement(speed);
       
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed += 0.1f * Time.deltaTime;
        }
        else
        {
            speed -= 0.1f * Time.deltaTime;
        }
        /* if (Input.GetKeyDown(KeyCode.E))
         {
             StartCoroutine(Rutin());
         }*/
        #endregion
        if(unlockGate == true && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        Charge = Mathf.Clamp(Charge, 0, 100);
        if(hasKey == true && battery >= 3)
        {
            unlockGate = true;
        }
        if( Charge <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void Movement(float movementSpeed)
    {
        float moveZ = Input.GetAxis("Vertical"); //W-S
        float moveX = Input.GetAxis("Horizontal");//A-D
        /* moveDirection = new Vector3(moveX * movementSpeed * Time.deltaTime, 0, moveZ * movementSpeed * Time.deltaTime);
         transform.position += moveDirection;*/
        moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
    }
   
    /*  IEnumerator Rutin()
      {
          canDegeri = 100;

          yield return new WaitForSeconds(3f);
          canDegeri = 50;
          yield return new WaitForSeconds(5f);
          canDegeri = 100;
      }*/
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Gate"))
        {
            
            if (hasKey && battery >=3)
            {
                unlockGate = true;
            }
        }
        if (col.CompareTag("Battery"))
        {
            battery ++;
            Destroy(col.gameObject);
           Charge += 15f;
        }
        if (col.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(col.gameObject);
        }
    }
    private void OnTriggerExit(Collider col)

    {
        if (col.CompareTag("Gate"))
        {

            if (hasKey && battery >= 3)
            {
                unlockGate = false;
            }
        }
    }

}
