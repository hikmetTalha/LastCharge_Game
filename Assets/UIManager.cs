using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Slider healthSlider;
    private CharacterMovement playerScript;
    public TextMeshProUGUI batteryText;
    
 

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerScript = player.GetComponent<CharacterMovement>();
            healthSlider.maxValue = 100f;
        }
    }


    void Update()
    {
        if(playerScript != null)
        {
            healthSlider.value = playerScript.Charge;
            if (batteryText != null)
            {
                batteryText.text = "Batteries " + playerScript.battery.ToString() + "  / 3";
            }
        }
       
    }
}
