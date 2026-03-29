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
    private Animator anim;
    private bool isDead = false;
    private bool canTakeDamage = true;

    [Header("Character Traits")]

    [Tooltip("Karakterin hýzýdýr.")] public float speed;
   public AudioClip robotDeath;
   public AudioClip robotStep;
   public AudioClip getKey;
   public AudioClip doorOpen;
   public AudioSource SFXsource;


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
        anim = GetComponent<Animator>();
        SFXsource = GetComponent<AudioSource>();
    }

    void Update()
  
    {
        if (isDead) return;
        #region MOVEMENT_PART
        Charge -= 2f*Time.deltaTime;
        speed = Mathf.Clamp(speed , 10f, 15f);
        Movement(speed);
      
      
        #endregion
        bool isWalking = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        anim.SetFloat("speed", isWalking ? 1f : 0f);
        if (isWalking)
        {
            if (!SFXsource.isPlaying && robotStep != null)
            {
                SFXsource.clip = robotStep;
                SFXsource.loop = true;
                SFXsource.Play();
            }
        }
        else
        {
            if (SFXsource.isPlaying && SFXsource.clip == robotStep)
            {
                SFXsource.Stop();
            }
        }
        if (unlockGate == true && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenDoorRoutine());
        }
        Charge = Mathf.Clamp(Charge, 0, 100);
        if(hasKey == true && battery >= 3)
        {
            unlockGate = true;
        }
        if (Charge <= 0 && !isDead)
        {
            StartCoroutine(DeathRoutine());
        }
    }
    void Movement(float movementSpeed)
    {
        float moveZ = Input.GetAxis("Vertical"); 
        float moveX = Input.GetAxis("Horizontal");
        
        moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        controller.Move(moveDirection * movementSpeed * Time.deltaTime);
        
    }

    public void PlayFootstep()
    {

        if (!isDead && robotStep != null)
        {
            SFXsource.PlayOneShot(robotStep);
        }
    }
   
    
    private void OnTriggerEnter(Collider col)
    {
        if (isDead) return;
        if (col.CompareTag("Gate"))
        {
            
            if (hasKey && battery >=3)
            {
                unlockGate = true;
            }
        }
        if (col.CompareTag("AI") && canTakeDamage)
        {
            StartCoroutine(TakeDamageRoutine());
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

            if (SFXsource.clip == robotStep)
            {
                SFXsource.Stop();
            }

            SFXsource.PlayOneShot(getKey);
        }
        

    }
    IEnumerator DeathRoutine()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        if (SFXsource.isPlaying && SFXsource.clip == robotStep)
        {
            SFXsource.Stop();
        }
        SFXsource.PlayOneShot(robotDeath);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    IEnumerator TakeDamageRoutine()
    {
        canTakeDamage = false; 
        Charge -= 30f;
        anim.SetTrigger("isGetHit");

        yield return new WaitForSeconds(1.0f); 
        canTakeDamage = true; 
    }

    IEnumerator OpenDoorRoutine()
    {
        if (SFXsource.isPlaying && SFXsource.clip == robotStep)
        {
            SFXsource.Stop();
        }
        if (doorOpen != null)
        {
            SFXsource.PlayOneShot(doorOpen);
        }
        yield return new WaitForSeconds(1.5f);      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
