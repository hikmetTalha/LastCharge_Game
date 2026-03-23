using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    public float Charge = 100;
    public int battery = 0;
   public bool hasKey = false;
    bool unlockGate = false;
    private CharacterController controller;

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
        controller = GetComponent<CharacterController>();
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
        
        moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        controller.Move(moveDirection * movementSpeed * Time.deltaTime);
    }
   
    
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
