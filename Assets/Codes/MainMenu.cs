using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_01");
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Oyundan çýkýldý.");
    }

}
