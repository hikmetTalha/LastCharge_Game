using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursorLock : MonoBehaviour
{
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Debug.Log(" Oyun Kapatýlýyor.");
        Application.Quit(); 
    }
    
}
